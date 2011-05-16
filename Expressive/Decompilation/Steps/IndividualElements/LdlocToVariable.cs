using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdlocToVariable : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<Instruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<Instruction, int>> {
            { OpCodes.Ldloc_0, _ => 0 },
            { OpCodes.Ldloc_1, _ => 1 },
            { OpCodes.Ldloc_2, _ => 2 },
            { OpCodes.Ldloc_3, _ => 3 },
            { OpCodes.Ldloc,   x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Ldloc_S, x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Ldloca,  x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Ldloca_S,  x => ((VariableReferenceInstruction)x).Ordinal }
        };

        private MethodBody methodBody;

        public override void Initialize(DecompilationContext context) {
            this.methodBody = context.Method.GetMethodBody();
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return variableIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var indexGetter = variableIndexGetters[instruction.OpCode];
            var variableIndex = indexGetter(instruction.Instruction);
            
            return new ExpressionElement(new LocalExpression(
                variableIndex, this.methodBody.LocalVariables[variableIndex].LocalType
            ));
        }
    }
}
