using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using ClrTest.Reflection;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class CallToExpression : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Call;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var method = ((InlineMethodInstruction)instruction.Instruction).Method;
            return new ExpressionElement(IdentifyAndCollectCall(method, context));
        }

        private Expression IdentifyAndCollectCall(MethodBase methodBase, IndividualDecompilationContext context) {
            var parameters = methodBase.GetParameters();
            var target = !methodBase.IsStatic
                       ? context.CapturePreceding<ExpressionElement>(-parameters.Length-1).Expression
                       : null;

            var property = GetProperty(methodBase);
            if (property != null)
                return Expression.Property(target, property);

            var method = methodBase as MethodInfo;
            if (method == null)
                throw new NotImplementedException("Only method and property calls are implemented.");

            context.VerifyPrecedingCount(parameters.Length, (actualCount, precedingString) => string.Format(
                "Method {0} expects {1} parameters." + Environment.NewLine +
                "However, there are only {2} preceding elements: " + Environment.NewLine + "{3}",
                    methodBase, parameters.Length, actualCount, precedingString
            ));

            var arguments = new List<Expression>();
            while (arguments.Count < parameters.Length) {
                arguments.Add(context.CapturePreceding<ExpressionElement>().Expression);
            }
            arguments.Reverse();

            return Expression.Call(target, method, arguments);
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
