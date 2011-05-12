using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public abstract class BranchingAwareStepBase : IDecompilationStep {
        public virtual void Apply(IList<IElement> elements, DecompilationContext context) {
            foreach (var tree in elements.OfType<IBranchingElement>()) {
                foreach (var branch in tree.GetBranches()) {
                    this.Apply(branch, context);
                }
            }
            this.ApplyToSpecificBranch(elements, context);
        }

        protected abstract void ApplyToSpecificBranch(IList<IElement> elements, DecompilationContext context);
    }
}
