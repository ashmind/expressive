using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class StelemToAssignment : ElementInterpretation<InstructionElement, ArrayItemAssignmentElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode.Name.StartsWith(OpCodes.Stelem.Name);
        }

        public override ArrayItemAssignmentElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var value = context.CapturePreceding();
            var index = context.CapturePreceding();
            var array = context.CapturePreceding();

            return new ArrayItemAssignmentElement(array, index, value);
        }
    }
}
