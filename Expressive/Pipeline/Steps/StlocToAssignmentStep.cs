using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class StlocToAssignmentStep : BranchingAwareStepBase {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Stloc_0, _ => 0 },
            { OpCodes.Stloc_1, _ => 1 },
            { OpCodes.Stloc_2, _ => 2 },
            { OpCodes.Stloc_3, _ => 3 },
            { OpCodes.Stloc_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Stloc,   x => ((InlineVarInstruction)x).Ordinal }
        };

        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                var instruction = elements[i] as InstructionElement;
                if (instruction == null)
                    continue;

                var indexGetter = variableIndexGetters.GetValueOrDefault(instruction.OpCode);
                if (indexGetter == null)
                    continue;

                var previous = elements[i - 1];
                elements.RemoveAt(i - 1);
                i -= 1;

                elements[i] = new VariableAssignmentElement(indexGetter(instruction.Instruction), previous);
            }
        }
    }
}
