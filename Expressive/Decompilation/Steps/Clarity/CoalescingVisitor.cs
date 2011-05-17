using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions.Matchers;

namespace Expressive.Decompilation.Steps.Clarity {
    public class CoalescingVisitor : ElementVisitor {
        protected override Expression VisitConditional(ConditionalExpression c) {
            c = (ConditionalExpression)base.VisitConditional(c);

            return TryToCoalesceAsObjects(c)
                ?? TryToCoalesceAsNullable(c)
                ?? c;
        }

        private Expression TryToCoalesceAsNullable(ConditionalExpression c) {
            var hasValueTarget = (Expression)null;
            var getValueTarget = (Expression)null;

            var matched = Matcher
                .For(c.Test).AsPropertyOrField()
                    .Property(property => property.Name == "HasValue" && property.DeclaringType.IsGenericTypeDefinedAs(typeof(Nullable<>)))
                    .Do(p => hasValueTarget = p.Expression)

                .For(c.IfTrue).AsConvert()
                    .Type(typeof(Nullable<>))
                    .Operand().AsMethodCall()
                        .Method(method => method.Name == "GetValueOrDefault"
                                       && method.DeclaringType.IsGenericTypeDefinedAs(typeof(Nullable<>)))
                        .Do(call => getValueTarget = call.Object)

                .Matched;

            if (!matched)
                return null;

            if (hasValueTarget != getValueTarget)
                return null;

            return Expression.Coalesce(getValueTarget, c.IfFalse);
        }


        private static Expression TryToCoalesceAsObjects(ConditionalExpression c) {
            var testPart = (Expression)null;
            var matched = Matcher
                .For(c.Test)
                .OneOf(ExpressionType.Equal, ExpressionType.NotEqual)
                .AsBinary()
                .LeftOrRight(
                    leftOrRight => leftOrRight.Constant(v => v == null),
                    other => testPart = other
                )
                .Matched;

            if (!matched)
                return null;

            if (testPart == c.IfTrue && c.Test.NodeType == ExpressionType.NotEqual)
                return Expression.Coalesce(testPart, c.IfFalse);

            if (testPart == c.IfFalse && c.Test.NodeType == ExpressionType.Equal)
                return Expression.Coalesce(testPart, c.IfTrue);

            return null;
        }
    }
}
