using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Expressive.Disassembly.Instructions;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class StfldToAssignment : ElementInterpretation<InstructionElement, MemberAssignmentElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Stfld
                || instruction.OpCode == OpCodes.Stsfld;
        }

        public override MemberAssignmentElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var field = ((FieldReferenceInstruction)instruction.Instruction).Field;
            var value = context.CapturePreceding();
            var instance = (Expression)null;
            if (!field.IsStatic)
                instance = context.CapturePreceding();

            value = BooleanSupport.ConvertIfRequired(value, field.FieldType);
            return new MemberAssignmentElement(instance, field, value);
        }
    }
}
