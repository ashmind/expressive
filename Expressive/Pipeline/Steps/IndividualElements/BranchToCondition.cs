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
    public class BranchToCondition : ElementInterpretation<ConditionalBranchElement, IElement> {
        private static readonly IDictionary<string, Func<Expression, Expression>> unary = new Dictionary<string, Func<Expression, Expression>> {
            { OpCodes.Brtrue.Name,  e => e },
            { OpCodes.Brfalse.Name, e => Expression.Not(e) },
        };

        private static readonly IDictionary<string, Func<Expression, Expression, Expression>> binary = new Dictionary<string, Func<Expression, Expression, Expression>> {
            { OpCodes.Beq.Name, Expression.Equal },
            { OpCodes.Ble.Name, Expression.LessThanOrEqual },
        };

        public override IElement Interpret(ConditionalBranchElement branch, IndividualInterpretationContext context) {
            var condition = CaptureCondition(branch, context);
            var targetAsExpression = AsSingleExpression(branch.Target);
            var fallbackAsExpression = AsSingleExpression(branch.Fallback);

            if (condition.Type != typeof(bool))
                condition = new BooleanAdapterExpression(condition);

            if (targetAsExpression != null && fallbackAsExpression != null) {
                BooleanAdapterExpression.AdaptIfRequired(ref targetAsExpression, ref fallbackAsExpression);
                return new ExpressionElement(Expression.Condition(condition, targetAsExpression, fallbackAsExpression));
            }

            var ifTrue = branch.Target;
            var ifFalse = branch.Fallback;
            if (ifTrue.Count == 0) {
                condition = Invert(condition);
                ifFalse = branch.Target;
                ifTrue = branch.Fallback;
            }

            return new IfThenElement(condition, ifTrue, ifFalse);
        }

        private Expression Invert(Expression condition) {
            if (condition.NodeType == ExpressionType.Not)
                return ((UnaryExpression) condition).Operand;

            return Expression.Not(condition);
        }

        private Expression CaptureCondition(ConditionalBranchElement branch, IndividualInterpretationContext context) {
            var rootOpCodeName = branch.OpCode.Name.SubstringBefore(".");
            var isUnary = unary.ContainsKey(rootOpCodeName);
            if (isUnary) {
                var operand = context.CapturePreceding<ExpressionElement>().Expression;
                return unary[rootOpCodeName].Invoke(operand);
            }

            var right = context.CapturePreceding<ExpressionElement>().Expression;
            var left = context.CapturePreceding<ExpressionElement>().Expression;
            return binary[rootOpCodeName].Invoke(left, right);
        }

        private Expression AsSingleExpression(IList<IElement> elements) {
            if (elements.Count == 0)
                return null;

            if (elements.Count > 1)
                return null;

            var expressionElement = elements[0] as ExpressionElement;
            return expressionElement != null ? expressionElement.Expression : null;
        }
    }
}
