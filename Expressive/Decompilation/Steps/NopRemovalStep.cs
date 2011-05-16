using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class NopRemovalStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificElement(ref int index, IList<IElement> elements, Stack<BranchStackFrame> branchStack, DecompilationContext context) {
            var instruction = elements[index] as InstructionElement;
            if (instruction != null && instruction.OpCode == OpCodes.Nop) {
                elements.RemoveAt(index);
                index -= 1;
            }
        }
    }
}