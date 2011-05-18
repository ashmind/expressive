using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AshMind.Extensions;

namespace Expressive.Matching {
    public static class MatcherMethodCallExtensions {
        public static Matcher<MethodCallExpression> Method(
            this Matcher<MethodCallExpression> matcher,
            Func<MethodInfo, bool> matchMethod
        ) {
            return matcher.Match(target => matchMethod(target.Method));
        }

        public static Matcher<Expression> Argument(this Matcher<MethodCallExpression> matcher, int index) {
            return matcher.Match(n => index >= 0 && index <= n.Arguments.Count - 1)
                          .For(n => n.Arguments[index]);
        }

        public static Matcher<Expression> Argument<TParameter>(this Matcher<MethodCallExpression> matcher) {
            return matcher.For(n => {
                var parameters = n.Method.GetParameters();
                for (var i = 0; i < parameters.Length; i++) {
                    if (parameters[i].ParameterType == typeof(TParameter))
                        return n.Arguments[i];
                }

                return null;
            });
        }

        public static Matcher<MethodCallExpression> Argument<TParameter>(this Matcher<MethodCallExpression> matcher, Func<Matcher<Expression>, Matcher> matchArgument) {
            return matcher.Match(_ => matchArgument(matcher.Argument<TParameter>()).Matched);
        }
    }
}
