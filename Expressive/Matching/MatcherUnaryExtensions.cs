using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Matching;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherUnaryExtensions {
        public static Matcher<Expression> Operand(this Matcher<UnaryExpression> matcher) {
            return matcher.For(u => u.Operand);
        }
    }
}
