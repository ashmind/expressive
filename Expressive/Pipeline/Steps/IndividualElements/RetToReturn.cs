using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class RetToReturn : ElementInterpretation<InstructionElement, ReturnElement> {
        private Type returnType;

        public override void Initialize(InterpretationContext context) {
            var method = context.Method as MethodInfo;
            this.returnType = method != null ? method.ReturnType : typeof(void);

            base.Initialize(context);
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ret;
        }

        public override ReturnElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var result = this.returnType != typeof(void)
                       ? context.CapturePreceding<ExpressionElement>().Expression
                       : null;

            result = BooleanAdapterExpression.AdaptIfRequired(result, this.returnType);
            return new ReturnElement(result);
        }
    }
}
