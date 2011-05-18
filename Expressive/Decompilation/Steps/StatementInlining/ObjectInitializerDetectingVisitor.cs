using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class ObjectInitializerDetectingVisitor : ElementVisitor {
        public override void VisitList(IList<IElement> elements) {
            base.VisitList(elements);
            for (var i = elements.Count - 1; i >= 0; i--) {
                NewExpression @new;
                VariableAssignmentElement assignment;
                var matched = Matcher
                    .For(elements[i]).As<VariableAssignmentElement>()
                        .AssignTo(out assignment)
                    .For(v => v.Value).As<NewExpression>()
                        .AssignTo(out @new)
                        .Matched;

                if (!matched)
                    continue;

                var initializer = this.CollectInitializer(@new, assignment.VariableIndex, i, elements);
                if (initializer != null)
                    assignment.Value = initializer;
            }
        }

        private MemberInitExpression CollectInitializer(NewExpression @new, int variableIndex, int elementIndex, IList<IElement> elements) {
            var bindings = new List<MemberBinding>();
            for (var i = elementIndex + 1; i < elements.Count; i++) {
                var binding = (MemberBinding)null;
                var matched = Matcher
                    .For(elements[i]).As<MemberAssignmentElement>()
                        .Do(a => binding = Expression.Bind(a.Member, a.Value))
                    .For(m => m.Instance).As<LocalExpression>()
                        .Match(l => l.Index == variableIndex)
                    .Matched;

                if (!matched)
                    break;

                bindings.Add(binding);
                elements.RemoveAt(i);
                i -= 1;
            }

            if (bindings.Count == 0)
                return null;

            return Expression.MemberInit(@new, bindings);
        }
    }
}
