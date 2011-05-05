using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class StlocToAssignmentStep : IInterpretationStep {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Stloc_0, _ => 0 },
            { OpCodes.Stloc_1, _ => 1 },
            { OpCodes.Stloc_2, _ => 2 },
            { OpCodes.Stloc_3, _ => 3 },
            { OpCodes.Stloc_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Stloc,   x => ((InlineVarInstruction)x).Ordinal }
        };

        public void Apply(InterpretationWorkspace workspace) {
            for (var i = 0; i < workspace.Elements.Count; i++) {
                var instruction = workspace.Elements[i] as InstructionElement;
                if (instruction == null)
                    continue;

                var indexGetter = variableIndexGetters.GetValueOrDefault(instruction.OpCode);
                if (indexGetter == null)
                    continue;

                var previous = workspace.Elements[i - 1];
                workspace.Elements.RemoveAt(i - 1);
                i -= 1;

                workspace.Elements[i] = new VariableAssignmentElement(indexGetter(instruction.Instruction), previous);
            }
        }
    }
}
