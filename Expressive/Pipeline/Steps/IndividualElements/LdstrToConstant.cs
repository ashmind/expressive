using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using ClrTest.Reflection;

using Expressive.Elements;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class LdstrToConstant : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ldstr;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            return new ExpressionElement(Expression.Constant(
                ((InlineStringInstruction)instruction.Instruction).String
            ));
        }
    }
}
