using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class UnconditionalBranchesRemovalStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificElement(ref int index, IList<IElement> elements, Stack<BranchStackFrame> branchStack, DecompilationContext context) {
            var branching = elements[index] as BranchingElement;
            if (branching != null && (branching.OpCode == OpCodes.Br || branching.OpCode == OpCodes.Br_S)) {
                elements.RemoveAt(index);
                index -= 1;
            }
        }
    }
}
