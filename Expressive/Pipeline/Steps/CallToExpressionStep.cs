using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class CallToExpressionStep : IInterpretationStep {
        public void Apply(InterpretationWorkspace workspace) {
            for (var i = 0; i < workspace.Elements.Count; i++) {
                var instruction = workspace.Elements[i] as InstructionElement;
                if (instruction == null || instruction.OpCode != OpCodes.Call)
                    continue;

                var method = ((InlineMethodInstruction)instruction.Instruction).Method;
                workspace.Elements[i] = new ExpressionElement(IdentifyAndCollectCall(method, i, workspace));
                workspace.Elements.RemoveWhere(e => e == null);
                i = 0; // restarting since we essentially recreated the collection
            }
        }

        private Expression IdentifyAndCollectCall(MethodBase methodBase, int elementIndex, InterpretationWorkspace workspace) {
            var parameters = methodBase.GetParameters();
            var target = !methodBase.IsStatic
                       ? CollectExpression(workspace, elementIndex - parameters.Length - 1)
                       : null;

            var property = GetProperty(methodBase);
            if (property != null)
                return Expression.Property(target, property);

            var method = methodBase as MethodInfo;
            if (method == null)
                throw new NotImplementedException("Only method and property calls are implemented.");

            var arguments = Enumerable.Range(elementIndex - parameters.Length, parameters.Length)
                                      .Select(index => CollectExpression(workspace, index));

            return Expression.Call(target, method, arguments);
        }

        private Expression CollectExpression(InterpretationWorkspace workspace, int index) {
            var expressionElement = workspace.Elements[index] as ExpressionElement;
            if (expressionElement == null)
                throw new InvalidOperationException("Element " + workspace.Elements[index] + " must be an ExpressionElement to be used in call.");

            workspace.Elements[index] = null; // this will be removed in the end
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
