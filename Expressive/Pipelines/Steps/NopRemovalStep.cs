using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipelines.Steps {
    public class NopRemovalStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            elements.RemoveWhere(e => {
                var instruction = e as InstructionElement;
                return instruction != null && instruction.OpCode == OpCodes.Nop;
            });
        }
    }
}