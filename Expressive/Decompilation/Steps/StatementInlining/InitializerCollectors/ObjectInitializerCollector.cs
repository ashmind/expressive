using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors {
    public class ObjectInitializerCollector : InitializerCollector<NewExpression, MemberAssignmentElement, MemberBinding> {
        protected override Matcher<MemberAssignmentElement> MatchFollowing(Matcher<MemberAssignmentElement> itemMatcher, int indexOffset, out MemberBinding binding) {
            var bindingFixed = (MemberBinding)null;
            var matcher = itemMatcher
                .Do(a => bindingFixed = Expression.Bind(a.Member, a.Value));

            binding = bindingFixed;
            return matcher;
        }

        protected override Expression GetVariable(MemberAssignmentElement rawElement) {
            return rawElement.Instance;
        }

        protected override Expression ToInitializer(NewExpression @new, IList<MemberBinding> bindings) {
            return Expression.MemberInit(@new, bindings);
        }
    }
}
