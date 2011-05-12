using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class CutBranchesRemovalStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, DecompilationContext context) {
            elements.RemoveWhere(e => e is CutBranchElement);
        }
    }
}
