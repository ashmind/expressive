using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements.Expressions;

namespace Expressive.Decompilation {
    public static class BooleanSupport {
        private class BooleanConvertingVisitor : ExpressionTreeVisitor {
            protected override Expression Visit(Expression exp) {
                return exp;
            }

            public Expression AttemptToConvert(Expression exp) {
                var converted = base.Visit(exp);
                if (converted.Type == typeof(bool))
                    return converted;

                if (exp.Type == typeof(int))
                    return exp.NotEqual(Expression.Constant(0));

                if (!exp.Type.IsValueType)
                    return exp.NotEqual(Expression.Constant(null));

                return exp;
            }
            
            protected override Expression VisitConstant(ConstantExpression constant) {
                return Expression.Constant(
                    constant.Value != null
                    && !Equals(constant.Value, 0)
                );
            }

            protected override Expression VisitConditional(ConditionalExpression conditional) {
                var test = this.AttemptToConvert(conditional.Test);
                var ifTrue = this.AttemptToConvert(conditional.IfTrue);
                var ifFalse = this.AttemptToConvert(conditional.IfFalse);

                if (test == conditional.Test && ifTrue == conditional.IfTrue && ifFalse == conditional.IfFalse)
                    return conditional;

                return Expression.Condition(test, ifTrue, ifFalse);
            }

            protected override Expression VisitBinary(BinaryExpression b) {
                if (b.NodeType != ExpressionType.Equal && b.NodeType != ExpressionType.NotEqual)
                    return base.VisitBinary(b);

                if (b.Left.Type != typeof(int) || b.Right.Type != typeof(int))
                    return base.VisitBinary(b);

                var left = this.AttemptToConvert(b.Left);
                var right = this.AttemptToConvert(b.Right);

                if (left == b.Left && right == b.Right)
                    return b;

                if (left.Type != typeof(bool) || right.Type != typeof(bool))
                    return b;

                return Expression.MakeBinary(b.NodeType, left, right);
            }
        }

        public static void ConvertIfRequired(ref Expression left, ref Expression right) {
            left = ConvertIfRequired(left, right.Type);
            right = ConvertIfRequired(right, left.Type);
        }

        public static Expression ConvertIfRequired(Expression expression, Type requiredType) {
            if (requiredType != typeof(bool))
                return expression;

            return expression.Type == typeof(bool)
                 ? expression
                 : ConvertToBoolean(expression);
        }

        private static Expression ConvertToBoolean(Expression expression) {
            var converted = new BooleanConvertingVisitor().AttemptToConvert(expression);
            if (converted.Type != typeof(bool))
                throw new InvalidOperationException("Could not convert type of " + expression + " to System.Boolean.");

            return converted;
        }
    }
}
