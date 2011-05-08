using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Pipeline.Steps {
    public class LdlocToVariableStep : BranchingAwareStepBase {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldloc_0, _ => 0 },
            { OpCodes.Ldloc_1, _ => 1 },
            { OpCodes.Ldloc_2, _ => 2 },
            { OpCodes.Ldloc_3, _ => 3 },
            { OpCodes.Ldloc_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldloc,   x => ((InlineVarInstruction)x).Ordinal }
        };

        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            var body = context.Method.GetMethodBody();
            for (var i = 0; i < elements.Count; i++) {
                var instruction = elements[i] as InstructionElement;
                if (instruction == null)
                    continue;

                var indexGetter = variableIndexGetters.GetValueOrDefault(instruction.OpCode);
                if (indexGetter == null)
                    continue;

                var variableIndex = indexGetter(instruction.Instruction);
                var local = body.LocalVariables[variableIndex];
                elements[i] = new ExpressionElement(
                    new LocalExpression(variableIndex, local.LocalType)
                );
            }
        }
    }
}
