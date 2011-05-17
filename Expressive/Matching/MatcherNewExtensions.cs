using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Elements.Expressions.Matchers;

namespace Expressive.Matching {
    public static class MatcherNewExtensions {
        public static Matcher<NewExpression> Constructor(
            this Matcher<NewExpression> matcher,
            Func<ConstructorInfo, bool> matchConstructor
        ) {
            return matcher.Match(target => matchConstructor(target.Constructor));
        }

        public static Matcher<Expression> Argument(this Matcher<NewExpression> matcher, int index) {
            return matcher.Match(n => index >= 0 && index <= n.Arguments.Count - 1)
                          .For(n => n.Arguments[index]);
        }
    }
}
