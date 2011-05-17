using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class DupToCopy : InstructionToExpression {
        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode == OpCodes.Dup;
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            return context.GetPreceding();
        }
    }
}
