using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherCollectionExtensions {
        public static Matcher<IList<T>> Count<T>(this Matcher<IList<T>> matcher, int count) {
            return matcher.Match(target => target.Count == count);
        }
    }
}
