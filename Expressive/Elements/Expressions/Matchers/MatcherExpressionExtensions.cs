using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

namespace Expressive.Elements.Expressions.Matchers {
    public static class MatcherExpressionExtensions {
        public static Matcher<TExpression> OneOf<TExpression>(this Matcher<TExpression> matcher, params ExpressionType[] types) 
            where TExpression : Expression
        {
            return matcher.Match(target => types.Contains(target.NodeType));
        }

        public static Matcher<BinaryExpression> AsBinary<TExpression>(this Matcher<TExpression> matcher)
            where TExpression : Expression
        {
            return matcher.As<BinaryExpression>();
        }

        public static Matcher<MethodCallExpression> AsMethodCall<TExpression>(this Matcher<TExpression> matcher)
            where TExpression : Expression
        {
            return matcher.As<MethodCallExpression>();
        }

        public static Matcher<MemberExpression> AsPropertyOrField<TExpression>(this Matcher<TExpression> matcher)
            where TExpression : Expression
        {
            return matcher.As<MemberExpression>();
        }

        public static Matcher<UnaryExpression> AsConvert<TExpression>(this Matcher<TExpression> matcher)
            where TExpression : Expression
        {
            return matcher.OneOf(ExpressionType.Convert, ExpressionType.ConvertChecked)
                          .As<UnaryExpression>();
        }

        public static Matcher<ConstantExpression> Constant<TExpression>(
            this Matcher<TExpression> matcher,
            Func<object, bool> matchValue
        )
        where TExpression : Expression
        {
            return matcher.As<ConstantExpression>().Match(c => matchValue(c.Value));
        }

        public static Matcher<TExpression> Type<TExpression>(this Matcher<TExpression> matcher, Type type)
            where TExpression : Expression
        {
            return matcher.Type(t => t == type || t.IsGenericTypeDefinedAs(type));
        }

        public static Matcher<TExpression> Type<TExpression>(this Matcher<TExpression> matcher, Func<Type, bool> matchType)
            where TExpression : Expression
        {
            return matcher.Match(target => matchType(target.Type));
        }
    }
}
