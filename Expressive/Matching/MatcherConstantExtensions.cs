using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Matching {
    public static class MatcherConstantExtensions {
        public static Matcher<T> Value<T>(this Matcher<ConstantExpression> matcher) {
            return matcher.For(c => c.Value).As<T>();
        }

        public static Matcher<ConstantExpression> Value(this Matcher<ConstantExpression> matcher, Func<object, bool> matchValue) {
            return matcher.Match(c => matchValue(c.Value));
        }

        public static Matcher<ConstantExpression> Value(this Matcher<ConstantExpression> matcher, object value) {
            return matcher.Match(c => Equals(c.Value, value));
        }

        public static Matcher<ConstantExpression> ValueIsNull(this Matcher<ConstantExpression> matcher) {
            return matcher.Match(c => c.Value == null);
        }
    }
}
