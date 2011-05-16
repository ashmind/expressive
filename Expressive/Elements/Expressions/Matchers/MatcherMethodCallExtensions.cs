using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherMethodCallExtensions {
        public static Matcher<MethodCallExpression> Method(
            this Matcher<MethodCallExpression> matcher,
            Func<MethodInfo, bool> matchMethod
        ) {
            return matcher.Match(target => matchMethod(target.Method));
        }
    }
}
