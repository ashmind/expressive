using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps.Clarity {
    using ConversionToOperator = Func<Expression, Expression, Expression, Expression>;

    // a ? true : b  =>  a || b
    // a ? false : b => !a && b
    // a ? b : true  => !a || b
    // a ? b : false =>  a && b
    public class ConditionToOperatorVisitor : ElementVisitor {
        private static readonly IDictionary<object, ConversionToOperator> table = new Dictionary<object, ConversionToOperator> {
            { Pair(true, null),  (a, _, b) => a.OrElse(b) },
            { Pair(false, null), (a, _, b) => Expression.Not(a).AndAlso(b) },
            { Pair(null, true),  (a, b, _) => Expression.Not(a).OrElse(b) },
            { Pair(null, false), (a, b, _) => a.AndAlso(b) }
        };

        protected override Expression VisitConditional(ConditionalExpression c) {
            c = (ConditionalExpression)base.VisitConditional(c);
            if (c.Type != typeof(bool))
                return c;

            var ifTrueAsConstant = c.IfTrue as ConstantExpression;
            var ifFalseAsConstant = c.IfFalse as ConstantExpression;
            if (ifTrueAsConstant == null && ifFalseAsConstant == null)
                return c;

            return table[Pair(Boolean(ifTrueAsConstant), Boolean(ifFalseAsConstant))].Invoke(
                c.Test, c.IfTrue, c.IfFalse
            );
        }

        private static bool? Boolean(ConstantExpression constant) {
            return constant != null ? (bool?)constant.Value : null;
        }

        private static object Pair(bool? a, bool? b) {
            return new { a, b };
        }
    }
}
