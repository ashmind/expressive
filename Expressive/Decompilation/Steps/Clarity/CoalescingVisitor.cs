using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.Clarity {
    public class CoalescingVisitor : ElementVisitor {
        protected override Expression VisitConditional(ConditionalExpression c) {
            c = (ConditionalExpression)base.VisitConditional(c);

            var testPart = (Expression)null;
            var matched = ExpressionMatcher
                            .Match(c.Test)
                            .OneOf(ExpressionType.Equal, ExpressionType.NotEqual)
                            .BinaryWith(
                                leftOrRight => leftOrRight.Constant(v => v == null),
                                other => testPart = other
                            )
                            .Matched;

            if (!matched)
                return c;

            if (testPart == c.IfTrue && c.Test.NodeType == ExpressionType.NotEqual)
                return Expression.Coalesce(testPart, c.IfFalse);

            if (testPart == c.IfFalse && c.Test.NodeType == ExpressionType.Equal)
                return Expression.Coalesce(testPart, c.IfTrue);

            return c;
        }
    }
}
