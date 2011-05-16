using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Decompilation.Steps {
    public class ExpressionMatcher {
        private readonly Expression expression;

        private ExpressionMatcher(Expression expression) {
            this.expression = expression;
            this.Matched = true;
        }

        public bool Matched { get; private set; }

        public static ExpressionMatcher Match(Expression expression) {
            return new ExpressionMatcher(expression);
        }

        public ExpressionMatcher OneOf(params ExpressionType[] types) {
            if (!this.Matched)
                return this;

            return this.MatchIf(types.Contains(this.expression.NodeType));
        }

        public ExpressionMatcher BinaryWith(
            Func<ExpressionMatcher, ExpressionMatcher> matchPart,
            Action<Expression> otherPart
        ) {
            return this.MatchAs<BinaryExpression>(b => {
                var leftMatched = matchPart(ExpressionMatcher.Match(b.Left)).Matched;
                if (leftMatched) {
                    otherPart(b.Right);
                    return true;
                }

                var rightMatched = matchPart(ExpressionMatcher.Match(b.Right)).Matched;
                if (rightMatched) {
                    otherPart(b.Left);
                    return true;
                }

                return false;
            });
        }

        public ExpressionMatcher Constant(Func<object, bool> matchValue) {
            return this.MatchAs<ConstantExpression>(c => matchValue(c.Value));
        }

        private ExpressionMatcher MatchAs<TExpression>(Func<TExpression, bool> match) 
            where TExpression : class
        {
            if (!this.Matched)
                return this;

            var typed = this.expression as TExpression;
            if (typed == null)
                return this.FailMatch();

            return this.MatchIf(match(typed));
        }

        private ExpressionMatcher MatchIf(bool condition) {
            this.Matched = this.Matched && condition;
            return this;
        }
        
        private ExpressionMatcher FailMatch() {
            this.Matched = false;
            return this;
        }
    }
}
