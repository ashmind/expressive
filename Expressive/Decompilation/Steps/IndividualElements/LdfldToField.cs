using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdfldToField : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ldfld
                || instruction.OpCode == OpCodes.Ldsfld;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var field = ((FieldReferenceInstruction)instruction.Instruction).Field;
            var instance = !field.IsStatic
                         ? context.CapturePreceding<ExpressionElement>().Expression
                         : null;
            return new ExpressionElement(Expression.Field(instance, field));
        }
    }
}
