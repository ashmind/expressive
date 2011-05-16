using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public abstract class BranchingAwareStepBase : IDecompilationStep {
        public virtual void Apply(IList<IElement> elements, DecompilationContext context) {
            ApplyRecursive(elements, new Stack<BranchStackFrame>(),  context);
        }

        private void ApplyRecursive(IList<IElement> elements, Stack<BranchStackFrame> branchStack, DecompilationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                var element = elements[i];
                var branching = element as BranchingElement;
                if (branching != null) {
                    foreach (var branch in branching.GetBranches()) {
                        var frame = new BranchStackFrame(elements, branching, i);
                        branchStack.Push(frame);
                        this.ApplyRecursive(branch, branchStack, context);
                        branchStack.Pop();

                        i = frame.CurrentIndex;
                    }
                }

                this.ApplyToSpecificElement(ref i, elements, branchStack, context);
            }
        }

        protected abstract void ApplyToSpecificElement(ref int index, IList<IElement> elements, Stack<BranchStackFrame> branchStack, DecompilationContext context);
    }
}
