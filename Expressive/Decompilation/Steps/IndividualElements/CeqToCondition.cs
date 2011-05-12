using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class CeqToCondition : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ceq;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var right = context.CapturePreceding<ExpressionElement>().Expression;
            var left = context.CapturePreceding<ExpressionElement>().Expression;

            BooleanSupport.ConvertIfRequired(ref left, ref right);

            return new ExpressionElement(Expression.Condition(
                Expression.Equal(left, right),
                Expression.Constant(1),
                Expression.Constant(0)
            ));
        }
    }
}
