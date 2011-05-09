using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using ClrTest.Reflection;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class LdlocToVariable : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldloc_0, _ => 0 },
            { OpCodes.Ldloc_1, _ => 1 },
            { OpCodes.Ldloc_2, _ => 2 },
            { OpCodes.Ldloc_3, _ => 3 },
            { OpCodes.Ldloc_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldloc,   x => ((InlineVarInstruction)x).Ordinal }
        };

        private MethodBody methodBody;

        public override void Initialize(InterpretationContext context) {
            this.methodBody = context.Method.GetMethodBody();
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return variableIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var indexGetter = variableIndexGetters[instruction.OpCode];
            var variableIndex = indexGetter(instruction.Instruction);
            
            return new ExpressionElement(new LocalExpression(
                variableIndex, this.methodBody.LocalVariables[variableIndex].LocalType
            ));
        }
    }
}
