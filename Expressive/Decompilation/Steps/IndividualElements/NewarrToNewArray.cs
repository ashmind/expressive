using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class NewarrToNewArray : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Newarr;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var type = ((TypeReferenceInstruction)instruction.Instruction).Type.MakeArrayType();
            return new ExpressionElement(
                new NewArrayWithSizeExpression(type, context.CapturePreceding())
            );
        }
    }
}
