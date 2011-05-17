using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Matching;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherBinaryExtensions {
        public static Matcher<BinaryExpression> LeftOrRight<TResultOfMatch>(
            this Matcher<BinaryExpression> matcher,
            Func<Matcher<Expression>, Matcher<TResultOfMatch>> matchLeftOrRight,
            Action<Expression> doWithOther
        ) {
            return matcher.Match(b => {
                var leftMatched = matchLeftOrRight(Matcher.For(b.Left)).Matched;
                if (leftMatched) {
                    doWithOther(b.Right);
                    return true;
                }

                var rightMatched = matchLeftOrRight(Matcher.For(b.Right)).Matched;
                if (rightMatched) {
                    doWithOther(b.Left);
                    return true;
                }

                return false;
            });
        }

        public static Matcher<BinaryExpression> LeftAndRight<TResultOfOne, TResultOfOther>(
            this Matcher<BinaryExpression> matcher,
            Func<Matcher<Expression>, Matcher<TResultOfOne>> matchOne,
            Func<Matcher<Expression>, Matcher<TResultOfOther>> matchOther
        ) {
            return matcher.Match(
                b => (matchOne(Matcher.For(b.Left)).Matched && matchOther(Matcher.For(b.Right)).Matched)
                  || (matchOne(Matcher.For(b.Right)).Matched && matchOther(Matcher.For(b.Left)).Matched)
            );
        }
    }
}
