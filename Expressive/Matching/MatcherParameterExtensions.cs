using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Matching {
    public static class MatcherParameterExtensions {
        public static Matcher<ParameterExpression> Name(this Matcher<ParameterExpression> matcher, string name) {
            return matcher.Match(target => matcher.Target.Name == name);
        }
    }
}
