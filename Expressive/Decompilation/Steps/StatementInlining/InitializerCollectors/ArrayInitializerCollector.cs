using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors {
    public class ArrayInitializerCollector : InitializerCollector<NewArrayExpression, ArrayItemAssignmentElement, Expression> {
        protected override Matcher<ArrayItemAssignmentElement> MatchFollowing(Matcher<ArrayItemAssignmentElement> itemMatcher, int indexOffset, out Expression expression) {
            var expressionFixed = (Expression)null;
            var matcher = itemMatcher
                .For(
                    e => e.Index,
                    m => m.AsConstant().Value<int>().Is(indexOffset)
                )
                .Do(a => expressionFixed = a.Value);

            expression = expressionFixed;
            return matcher;
        }

        protected override Expression GetVariable(ArrayItemAssignmentElement rawElement) {
            return rawElement.Array;
        }

        protected override Expression ToInitializer(NewArrayExpression @new, IList<Expression> values) {
            return Expression.NewArrayInit(@new.Type.GetElementType(), values);
        }
    }
}
