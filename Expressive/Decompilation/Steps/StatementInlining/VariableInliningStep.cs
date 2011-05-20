using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Decompilation.Steps.StatementInlining.AssignmentInlining;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class VariableInliningStep : IDecompilationStep {
        private readonly AssignmentInliner inliner;

        public VariableInliningStep(AssignmentInliner inliner) {
            this.inliner = inliner;
        }

        public virtual void Apply(IList<IElement> elements, DecompilationContext context) {
            this.inliner.Inline(elements, _ => true);
        }
    }
}
