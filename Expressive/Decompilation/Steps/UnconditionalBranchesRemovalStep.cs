using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class UnconditionalBranchesRemovalStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, DecompilationContext context) {
            elements.RemoveWhere(e => {
                var branching = e as BranchingElement;
                return (branching != null) && (
                    branching.OpCode == OpCodes.Br
                    ||
                    branching.OpCode == OpCodes.Br_S
                );
            });
        }
    }
}
