using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements.Expressions.Matchers;

namespace Expressive.Matching {
    public static class MatcherConstantExtensions {
        public static Matcher<object> Value(this Matcher<ConstantExpression> matcher) {
            return matcher.For(c => c.Value);
        }

        public static Matcher<ConstantExpression> Value(this Matcher<ConstantExpression> matcher, Func<object, bool> matchValue) {
            return matcher.Match(c => matchValue(c.Value));
        }

        public static Matcher<ConstantExpression> ValueIsNull(this Matcher<ConstantExpression> matcher) {
            return matcher.Match(c => c.Value == null);
        }
    }
}
