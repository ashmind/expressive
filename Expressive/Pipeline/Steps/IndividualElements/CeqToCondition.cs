using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class CeqToCondition : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ceq;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var left = context.CapturePreceding<ExpressionElement>(-2);
            var right = context.CapturePreceding<ExpressionElement>(-1);
            return new ExpressionElement(Expression.Condition(
                Expression.Equal(left.Expression, right.Expression),
                Expression.Constant(1),
                Expression.Constant(0)
            ));
        }
    }
}
