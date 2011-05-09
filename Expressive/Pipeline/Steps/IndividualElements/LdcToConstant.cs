using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class LdcToConstant : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, object> values = new Dictionary<OpCode, object> {
            { OpCodes.Ldc_I4_0, 0 }
        };

        public override bool CanInterpret(InstructionElement instruction) {
            return values.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            return new ExpressionElement(Expression.Constant(
                values[instruction.OpCode]
            ));
        }
    }
}
