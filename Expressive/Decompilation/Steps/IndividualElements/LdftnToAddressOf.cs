using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdftnToAddressOf : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ldftn;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var method = ((MethodReferenceInstruction)instruction.Instruction).Method;
            return new ExpressionElement(new AddressOfExpression(method));
        }
    }
}
