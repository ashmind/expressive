using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipelines.Steps.Clarity {
    using ExpressionConversion = Func<Expression, Expression, Expression, Expression>;

    public class ConditionImprovementVisitor : ElementVisitor {
        private static readonly IDictionary<object, ExpressionConversion> table = new Dictionary<object, ExpressionConversion> {
            // a ? true : b  =>  a || b
            // a ? false : b => !a && b
            // a ? b : true  => !a || b
            // a ? b : false =>  a && b           
            { Pair(true, null),  (a, _, b) => a.OrElse(b) },
            { Pair(false, null), (a, _, b) => Expression.Not(a).AndAlso(b) },
            { Pair(null, true),  (a, b, _) => Expression.Not(a).OrElse(b) },
            { Pair(null, false), (a, b, _) => a.AndAlso(b) },

            // a ? true : true   => true
            // a ? true : false  => a
            // a ? false : true  => !a
            // a ? false : false => false
            { Pair(true, true),   Just(Expression.Constant(true)) },
            { Pair(true, false),  Just(a => a) },
            { Pair(false, true),  Just(a => Expression.Not(a)) },
            { Pair(false, false), Just(Expression.Constant(false)) },
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

        private static ExpressionConversion Just(Expression constant) {
            return delegate { return constant; };
        }

        private static ExpressionConversion Just(Func<Expression, Expression> func) {
            return (a, _1, _2) => func(a);
        }
    }
}
