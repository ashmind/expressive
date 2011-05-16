using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class PopToMove : ElementInterpretation<InstructionElement, IElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Pop;
        }

        public override IElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            return context.CapturePreceding<IElement>();
        }
    }
}
