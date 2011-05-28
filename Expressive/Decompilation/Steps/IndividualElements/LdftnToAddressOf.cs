using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Expressive.Disassembly.Instructions;
using Expressive.Elements.Expressions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdftnToAddressOf : InstructionToExpression {
        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode == OpCodes.Ldftn;
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            var method = ((MethodReferenceInstruction)instruction).Method;
            return new AddressOfExpression(method);
        }
    }
}
