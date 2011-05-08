using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;
using Expressive.Common;
using Expressive.Elements;
using Expressive.Elements.Presentation;

namespace Expressive.Pipeline.Steps {
    public class CallToExpressionStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                var instruction = elements[i] as InstructionElement;
                if (instruction == null || instruction.OpCode != OpCodes.Call)
                    continue;

                var preceding = new PartialList<IElement>(elements, 0, i);
                context.ApplyPipeline(preceding);
                i = preceding.Count;

                var method = ((InlineMethodInstruction)instruction.Instruction).Method;
                elements[i] = new ExpressionElement(IdentifyAndCollectCall(method, i, elements));
                elements.RemoveWhere(e => e == null);

                i = 0; // restarting since we essentially recreated the collection
            }
        }

        private Expression IdentifyAndCollectCall(MethodBase methodBase, int elementIndex, IList<IElement> elements) {
            var parameters = methodBase.GetParameters();
            var target = !methodBase.IsStatic
                       ? CollectExpression(elements, elementIndex - parameters.Length - 1)
                       : null;

            var property = GetProperty(methodBase);
            if (property != null)
                return Expression.Property(target, property);

            var method = methodBase as MethodInfo;
            if (method == null)
                throw new NotImplementedException("Only method and property calls are implemented.");

            if (elementIndex < parameters.Length) {
                throw new InvalidOperationException(string.Format(
                    "Method {0} expects {1} parameters." + Environment.NewLine +
                    "However, there are only {2} preceding elements: " + Environment.NewLine + "{3}",
                        methodBase, parameters.Length, elementIndex, ElementHelper.ToString(elements.Take(elementIndex), Indent.FourSpaces)
                ));
            }

            var arguments = Enumerable.Range(elementIndex - parameters.Length, parameters.Length)
                                      .Select(index => CollectExpression(elements, index));

            return Expression.Call(target, method, arguments);
        }

        private Expression CollectExpression(IList<IElement> elements, int index) {
            var expressionElement = elements[index] as ExpressionElement;
            if (expressionElement == null)
                throw new InvalidOperationException("Element " + elements[index] + " must be an ExpressionElement to be used in call.");

            elements[index] = null; // this will be removed in the end
            return expressionElement.Expression;
        }

        private PropertyInfo GetProperty(MethodBase method) {
            if (!method.IsSpecialName)
                return null;

            var properties = method.DeclaringType.GetProperties(
                (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic
            );

            return properties.FirstOrDefault(p => p.GetAccessors().Any(a => a.MetadataToken == method.MetadataToken));
        }
    }
}
