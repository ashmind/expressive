using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors {
    public abstract class InitializerCollector<TNewExpression, TFollowingElement, TValue> : IInitializerCollector 
        where TNewExpression : Expression
        where TFollowingElement : IElement
    {
        public Type NewExpressionType {
            get { return typeof(TNewExpression); }
        }

        public Expression AttemptToCollect(TNewExpression @new, int variableIndex, int elementIndex, IList<IElement> elements) {
            var values = new List<TValue>();
            var indexOffset = 0;
            for (var i = elementIndex + 1; i < elements.Count; i++) {
                TValue value;
                var itemMatcher = MatchFollowing(Matcher.For(elements[i]).As<TFollowingElement>(), indexOffset, out value);
                var matched = itemMatcher.For(GetVariable).As<LocalExpression>()
                    .Match(l => l.Index == variableIndex)
                    .Matched;
                
                if (!matched)
                    break;

                values.Add(value);
                elements.RemoveAt(i);
                i -= 1;

                indexOffset += 1;
            }

            if (values.Count == 0)
                return null;

            return ToInitializer(@new, values);
        }

        protected abstract Matcher<TFollowingElement> MatchFollowing(Matcher<TFollowingElement> itemMatcher, int indexOffset, out TValue value);
        protected abstract Expression GetVariable(TFollowingElement rawElement);
        protected abstract Expression ToInitializer(TNewExpression newExpression, IList<TValue> values);

        #region IInitializerCollector Members
        
        Expression IInitializerCollector.AttemptToCollect(Expression @new, int variableIndex, int elementIndex, IList<IElement> elements) {
            return this.AttemptToCollect((TNewExpression)@new, variableIndex, elementIndex, elements);
        }

        #endregion
    }
}
