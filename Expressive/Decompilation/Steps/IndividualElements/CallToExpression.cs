using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Expressive.Disassembly.Instructions;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class CallToElement : ElementInterpretation<InstructionElement, IElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Call
                || instruction.OpCode == OpCodes.Callvirt;
        }

        public override IElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var method = ((MethodReferenceInstruction)instruction.Instruction).Method;
            return IdentifyAndCollectCall(method, context);
        }

        protected IElement IdentifyAndCollectCall(MethodBase methodBase, IndividualDecompilationContext context) {
            var parameters = methodBase.GetParameters();
            var captureTarget = !methodBase.IsStatic
                              ? (Func<Expression>)(context.CapturePreceding)
                              : () => null;

            bool isSetter;
            var property = GetProperty(methodBase, out isSetter);
            if (property != null) {
                if (isSetter) {
                    var value = context.CapturePreceding();
                    return new MemberAssignmentElement(captureTarget(), property, value);
                }

                return new ExpressionElement(Expression.Property(captureTarget(), property));
            }

            var method = methodBase as MethodInfo;
            if (method == null)
                throw new NotImplementedException("Only method and property calls are implemented.");

            var arguments = CaptureArguments(parameters, methodBase, context);
            return new ExpressionElement(Expression.Call(captureTarget(), method, arguments));
        }

        protected IEnumerable<Expression> CaptureArguments(ParameterInfo[] parameters, MethodBase methodBase, IndividualDecompilationContext context) {
            context.VerifyPrecedingCount(parameters.Length, (actualCount, precedingString) => string.Format(
                "Method {0} expects {1} parameters." + Environment.NewLine +
                "However, there are only {2} preceding elements: " + Environment.NewLine + "{3}",
                    methodBase, parameters.Length, actualCount, precedingString
            ));

            var arguments = new List<Expression>();
            while (arguments.Count < parameters.Length) {
                arguments.Add(context.CapturePreceding());
            }

            arguments.Reverse();
            for (var i = 0; i < arguments.Count; i++) {
                arguments[i] = BooleanSupport.ConvertIfRequired(arguments[i], parameters[i].ParameterType);
            }

            return arguments;
        }

        private PropertyInfo GetProperty(MethodBase method, out bool isSetter) {
            isSetter = false;
            if (!method.IsSpecialName)
                return null;

            var properties = method.DeclaringType.GetProperties(
                (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (var property in properties) {
                if (method == property.GetGetMethod())
                    return property;

                if (method == property.GetSetMethod()) {
                    isSetter = true;
                    return property;
                }
            }

            return null;
        }
    }
}
