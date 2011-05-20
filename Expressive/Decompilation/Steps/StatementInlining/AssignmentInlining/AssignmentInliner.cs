using System;
using System.Collections.Generic;
using System.Linq;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.StatementInlining.AssignmentInlining {
    public class AssignmentInliner {
        public void Inline(IList<IElement> elements, Func<int, bool> variableIndexPredicate) {
            var inlineable = AssignmentEstimatingVisitor.Estimate(elements, variableIndexPredicate);
            AssignmentInliningVisitor.Inline(elements, inlineable);
        }
    }
}
