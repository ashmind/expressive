using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public abstract class InstructionToExpression : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement element) {
            return this.CanInterpret(element.Instruction);
        }

        public override ExpressionElement Interpret(InstructionElement element, IndividualDecompilationContext context) {
            return new ExpressionElement(this.Interpret(element.Instruction, context));
        }

        public abstract bool CanInterpret(Instruction instruction);
        public abstract Expression Interpret(Instruction instruction, IndividualDecompilationContext context);
    }
}
