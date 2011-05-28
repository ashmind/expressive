using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Expressive.Disassembly.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class NewarrToNewArray : InstructionToExpression {
        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode == OpCodes.Newarr;
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            var type = ((TypeReferenceInstruction)instruction).Type;
            return Expression.NewArrayBounds(type, context.CapturePreceding());
        }
    }
}
