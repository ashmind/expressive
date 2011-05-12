using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Pipelines {
    public static class BooleanSupport {
        private class BooleanConvertingVisitor {
            public Expression Visit(Expression exp) {
                var constant = exp as ConstantExpression;
                if (constant != null)
                    return this.VisitConstant(constant);

                var conditional = exp as ConditionalExpression;
                if (conditional != null)
                    return this.VisitConditional(conditional);

                return exp;
            }

            private Expression VisitConstant(ConstantExpression constant) {
                return Expression.Constant(
                    constant.Value != null
                    && !Equals(constant.Value, 0)
                );
            }

            private Expression VisitConditional(ConditionalExpression conditional) {
                var test = this.Visit(conditional.Test);
                var ifTrue = this.Visit(conditional.IfTrue);
                var ifFalse = this.Visit(conditional.IfFalse);

                if (test == conditional.Test && ifTrue == conditional.IfTrue && ifFalse == conditional.IfFalse)
                    return conditional;

                return Expression.Condition(test, ifTrue, ifFalse);
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
            var converted = new BooleanConvertingVisitor().Visit(expression);
            if (converted.Type != typeof(bool))
                throw new InvalidOperationException("Could not convert type of " + expression + " to System.Boolean.");

            return converted;
        }
    }
}
