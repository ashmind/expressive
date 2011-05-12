using System;
using System.Collections.Generic;
using System.Linq;
using Expressive.Elements;

namespace Expressive.Pipelines.Steps {
    public abstract class BranchingAwareStepBase : IInterpretationStep {
        public virtual void Apply(IList<IElement> elements, InterpretationContext context) {
            foreach (var tree in elements.OfType<IBranchingElement>()) {
                foreach (var branch in tree.GetBranches()) {
                    this.Apply(branch, context);
                }
            }
            this.ApplyToSpecificBranch(elements, context);
        }

        protected abstract void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context);
    }
}
