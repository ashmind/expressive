using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using ClrTest.Reflection;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdstrToConstant : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ldstr;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            return new ExpressionElement(Expression.Constant(
                ((InlineStringInstruction)instruction.Instruction).String
            ));
        }
    }
}
