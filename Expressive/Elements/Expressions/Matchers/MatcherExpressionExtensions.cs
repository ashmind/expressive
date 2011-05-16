using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AshMind.Extensions;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherExpressionExtensions {
        public static Matcher<TExpression> OneOf<TExpression>(this Matcher<TExpression> matcher, params ExpressionType[] types) 
            where TExpression : Expression
        {
            return matcher.Match(target => types.Contains(target.NodeType));
        }

        public static Matcher<TExpression> BinaryWith<TExpression>(
            this Matcher<TExpression> matcher,
            Func<Matcher<Expression>, Matcher<Expression>> matchPart,
            Action<Expression> otherPart
        )
        where TExpression : Expression
        {
            return matcher.MatchAs<BinaryExpression>(b => {
                var leftMatched = matchPart(Matcher.Match(b.Left)).Matched;
                if (leftMatched) {
                    otherPart(b.Right);
                    return true;
                }

                var rightMatched = matchPart(Matcher.Match(b.Right)).Matched;
                if (rightMatched) {
                    otherPart(b.Left);
                    return true;
                }

                return false;
            });
        }

        public static Matcher<TExpression> MethodCall<TExpression>(
            this Matcher<TExpression> matcher,
            Func<Matcher<MethodCallExpression>, Matcher<MethodCallExpression>> matchCall
        )
        where TExpression : Expression
        {
            return matcher.MatchAs<MethodCallExpression>(m => matchCall(Matcher.Match(m)).Matched);
        }

        public static Matcher<TExpression> Convert<TExpression>(
            this Matcher<TExpression> matcher,
            Func<Matcher<UnaryExpression>, Matcher<UnaryExpression>> matchConvert
        )
        where TExpression : Expression
        {
            return matcher.OneOf(ExpressionType.Convert, ExpressionType.ConvertChecked)
                          .MatchAs<UnaryExpression>(c => matchConvert(Matcher.Match(c)).Matched);
        }

        public static Matcher<TExpression> Property<TExpression>(
            this Matcher<TExpression> matcher,
            Func<PropertyInfo, bool> matchProperty,
            Action<MemberExpression> process
        )
        where TExpression : Expression
        {
            return matcher.MatchAs<MemberExpression>(m => {
                var property = m.Member as PropertyInfo;
                var matched = property != null && matchProperty(property);
                if (matched)
                    process(m);

                return matched;
            });
        }

        public static Matcher<TExpression> Constant<TExpression>(
            this Matcher<TExpression> matcher,
            Func<object, bool> matchValue
        )
        where TExpression : Expression
        {
            return matcher.MatchAs<ConstantExpression>(c => matchValue(c.Value));
        }

        public static Matcher<TExpression> Type<TExpression>(this Matcher<TExpression> matcher, Type type)
            where TExpression : Expression
        {
            return matcher.Match(
                target => target.Type == type
                       || target.Type.IsGenericTypeDefinedAs(type)
            );
        }
    }
}
