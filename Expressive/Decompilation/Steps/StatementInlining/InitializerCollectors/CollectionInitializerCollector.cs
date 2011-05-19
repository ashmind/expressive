using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors {
    public class CollectionInitializerCollector : InitializerCollector<NewExpression, ExpressionElement, ElementInit> {
        public override bool MatchNew(NewExpression expression) {
            return expression.Constructor.DeclaringType.HasInterface<IEnumerable>()
                || base.MatchNew(expression);
        }

        protected override Matcher<ExpressionElement> MatchFollowing(Matcher<ExpressionElement> addMatcher, int indexOffset, out ElementInit add) {
            var addFixed = (ElementInit)null;
            var matcher = addMatcher
                .For(
                    e => e.Expression,
                    c => c.AsMethodCall()
                          .Method(m => m.Name == "Add")
                          .Do(m => addFixed = Expression.ElementInit(m.Method, m.Arguments))
                );

            add = addFixed;
            return matcher;
        }

        protected override Expression GetVariable(ExpressionElement rawElement) {
            var call = (MethodCallExpression)rawElement.Expression;
            return call.Object;
        }

        protected override Expression ToInitializer(NewExpression @new, IList<ElementInit> values) {
            return Expression.ListInit(@new, values);
        }
    }
}
