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
            var matched = Matcher.Match(c.Test)
                .Property(
                    property => property.Name == "HasValue" && property.DeclaringType.IsGenericTypeDefinedAs(typeof(Nullable<>)),
                    p => hasValueTarget = p.Expression
                )
                .Matched;

            if (!matched)
                return null;

            var getValueTarget = (Expression)null;
            matched = Matcher
                .Match(c.IfTrue)
                .Convert(convert => convert
                    .Type(typeof(Nullable<>))
                    .Operand(o => o
                        .MethodCall(call => call
                            .Method(method => method.Name == "GetValueOrDefault"
                                           && method.DeclaringType.IsGenericTypeDefinedAs(typeof(Nullable<>)))
                            .Do(x => getValueTarget = x.Object)
                        )
                    )
                )
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
                .Match(c.Test)
                .OneOf(ExpressionType.Equal, ExpressionType.NotEqual)
                .BinaryWith(
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
