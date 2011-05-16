using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class DupToCopy : ElementInterpretation<InstructionElement, IElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Dup;
        }

        public override IElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            return context.GetPreceding<IElement>();
        }
    }
}
