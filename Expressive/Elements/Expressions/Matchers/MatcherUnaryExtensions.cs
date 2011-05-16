using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherMethodCallUnaryExtensions {
        public static Matcher<UnaryExpression> Operand(
            this Matcher<UnaryExpression> matcher,
            Func<Matcher<Expression>, Matcher<Expression>> matchOperand
        ) {
            return matcher.Match(target => matchOperand(Matcher.Match(target.Operand)).Matched);
        }
    }
}
