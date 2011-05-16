using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherMethodCallUnaryExtensions {
        public static Matcher<Expression> Operand(this Matcher<UnaryExpression> matcher) {
            return matcher.Get(u => u.Operand);
        }
    }
}
