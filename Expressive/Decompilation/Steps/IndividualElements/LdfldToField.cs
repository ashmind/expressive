using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdfldToField : InstructionToExpression {
        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode == OpCodes.Ldfld
                || instruction.OpCode == OpCodes.Ldsfld;
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            var field = ((FieldReferenceInstruction)instruction).Field;
            var instance = !field.IsStatic
                         ? context.CapturePreceding()
                         : null;
            return Expression.Field(instance, field);
        }
    }
}
