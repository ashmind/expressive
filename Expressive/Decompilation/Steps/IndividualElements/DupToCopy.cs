using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class DupToCopy : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Dup;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            return new ExpressionElement(context.GetPreceding());
        }
    }
}
