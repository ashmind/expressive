using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class OtherToCondition : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<Expression, Expression, Expression>> comparisons = new Dictionary<OpCode, Func<Expression, Expression, Expression>> {
            { OpCodes.Beq, Expression.Equal },
        };

        public override bool CanInterpret(InstructionElement instruction) {
            return comparisons.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var right = context.CapturePreceding<ExpressionElement>(-1).Expression;
            var left = context.CapturePreceding<ExpressionElement>(-1).Expression;

            this.EnsureCompatibilityInOneDirection(ref left, ref right);
            this.EnsureCompatibilityInOneDirection(ref right, ref left);

            return new ExpressionElement(Expression.Condition(
                comparisons[instruction.OpCode].Invoke(left, right),
                Expression.Constant(1),
                Expression.Constant(0)
            ));
        }

        private void EnsureCompatibilityInOneDirection(ref Expression left, ref Expression right) {
            if (left.Type != typeof(bool))
                return;

            if (right.Type == typeof(bool))
                return;

            // left is bool, right is not bool (int?)
            right = new BooleanAdapterExpression(right);
        }
    }
}
