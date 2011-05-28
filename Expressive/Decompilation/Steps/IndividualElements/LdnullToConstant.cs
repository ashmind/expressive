using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Expressive.Disassembly.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdstrToConstant : InstructionToExpression {
        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode == OpCodes.Ldstr;
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            return Expression.Constant(
                ((ValueInstruction<string>)instruction).Value
            );
        }
    }
}
