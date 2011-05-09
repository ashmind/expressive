using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class CeqToCondition : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ceq;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var left = context.CapturePreceding<ExpressionElement>(-2);
            var right = context.CapturePreceding<ExpressionElement>(-1);

            this.EnsureCompatibilityInOneDirection(left, right);
            this.EnsureCompatibilityInOneDirection(right, left);

            return new ExpressionElement(Expression.Condition(
                Expression.Equal(left.Expression, right.Expression),
                Expression.Constant(1),
                Expression.Constant(0)
            ));
        }

        private void EnsureCompatibilityInOneDirection(ExpressionElement left, ExpressionElement right) {
            if (left.Expression.Type != typeof(bool))
                return;

            if (right.Expression.Type == typeof(bool))
                return;

            // left is bool, right is not bool (int?)
            right.Expression = new BooleanAdapterExpression(right.Expression);

            //if (right.Expression.Type != typeof(int))
            //    throw new NotSupportedException("Cannot convert " + right.Expression.Type + " to System.Boolean.");

            //// need to rewrite
            //var constant = right.Expression as ConstantExpression;
            //right = constant == null 
            //      ? new ExpressionElement(right.Expression.NotEqual(Expression.Constant(0)))
            //      : new ExpressionElement(Expression.Constant(!Equals(constant.Value, 0)));
        }
    }
}
