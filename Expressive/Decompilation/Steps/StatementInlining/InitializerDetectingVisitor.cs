using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Decompilation.Steps.StatementInlining.AssignmentInlining;
using Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors;
using Expressive.Elements;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class InitializerDetectingVisitor : ElementVisitor {
        private readonly AssignmentInliner inliner;
        private readonly IInitializerCollector[] collectors;

        public InitializerDetectingVisitor(AssignmentInliner inliner, params IInitializerCollector[] collectors) {
            this.inliner = inliner;
            this.collectors = collectors;
        }

        public override void VisitList(IList<IElement> elements) {
            base.VisitList(elements);
            for (var i = elements.Count - 1; i >= 0; i--) {
                foreach (var collector in this.collectors) {
                    if (TryWith(collector, i, elements))
                        break;
                }
            }
        }

        protected virtual bool TryWith(IInitializerCollector collector, int index, IList<IElement> elements) {
            Expression @new;
            VariableAssignmentElement assignment;
            var matched = Matcher
                .For(elements[index]).As<VariableAssignmentElement>()
                .AssignTo(out assignment)
                .For(v => v.Value).Match(collector.MatchNew)
                    .AssignTo(out @new)
                .Matched;

            if (!matched)
                return false;

            var initializer = collector.AttemptToCollect(@new, assignment.VariableIndex, index, elements);
            if (initializer != null) {
                assignment.Value = initializer;
                this.inliner.Inline(elements, i => i == assignment.VariableIndex);
                return true;
            }

            return false;
        }
    }
}
