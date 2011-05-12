using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipelines.Steps {
    public class CutBranchesRemovalStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            elements.RemoveWhere(e => e is CutBranchElement);
        }
    }
}
