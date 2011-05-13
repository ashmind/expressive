using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class StlocToAssignment : ElementInterpretation<InstructionElement, VariableAssignmentElement> {
        private static readonly IDictionary<OpCode, Func<Instruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<Instruction, int>> {
            { OpCodes.Stloc_0, _ => 0 },
            { OpCodes.Stloc_1, _ => 1 },
            { OpCodes.Stloc_2, _ => 2 },
            { OpCodes.Stloc_3, _ => 3 },
            { OpCodes.Stloc_S, x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Stloc,   x => ((VariableReferenceInstruction)x).Ordinal }
        };

        private MethodBody methodBody;

        public override void Initialize(DecompilationContext context) {
            this.methodBody = context.Method.GetMethodBody();
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return variableIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override VariableAssignmentElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var value = context.CapturePreceding<ExpressionElement>().Expression;
            var index = variableIndexGetters[instruction.OpCode](instruction.Instruction);
            var type = this.methodBody.LocalVariables[index].LocalType;

            value = BooleanSupport.ConvertIfRequired(value, type);

            return new VariableAssignmentElement(index, value);
        }
    }
}
