using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Pipeline.Steps {
    public class LdlocToVariableStep : IInterpretationStep {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldloc_0, _ => 0 },
            { OpCodes.Ldloc_1, _ => 1 },
            { OpCodes.Ldloc_2, _ => 2 },
            { OpCodes.Ldloc_3, _ => 3 },
            { OpCodes.Ldloc_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldloc,   x => ((InlineVarInstruction)x).Ordinal }
        };

        public void Apply(InterpretationWorkspace workspace) {
            for (var i = 0; i < workspace.Elements.Count; i++) {
                var instruction = workspace.Elements[i] as InstructionElement;
                if (instruction == null)
                    continue;

                var indexGetter = variableIndexGetters.GetValueOrDefault(instruction.OpCode);
                if (indexGetter == null)
                    continue;

                var variableIndex = indexGetter(instruction.Instruction);
                var local = workspace.Method.GetMethodBody().LocalVariables[variableIndex];
                workspace.Elements[i] = new ExpressionElement(
                    new LocalExpression(variableIndex, local.LocalType)
                );
            }
        }
    }
}
