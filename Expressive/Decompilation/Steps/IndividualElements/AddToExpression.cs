using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class AddToExpression : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Add;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var right = context.CapturePreceding<ExpressionElement>().Expression;
            var left = context.CapturePreceding<ExpressionElement>().Expression;

            return new ExpressionElement(Expression.Add(left, right));
        }
    }
}
