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
    public class StlocToAssignment : ElementInterpretation<InstructionElement, VariableAssignmentElement> {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> variableIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Stloc_0, _ => 0 },
            { OpCodes.Stloc_1, _ => 1 },
            { OpCodes.Stloc_2, _ => 2 },
            { OpCodes.Stloc_3, _ => 3 },
            { OpCodes.Stloc_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Stloc,   x => ((InlineVarInstruction)x).Ordinal }
        };

        private MethodBody methodBody;

        public override void Initialize(InterpretationContext context) {
            this.methodBody = context.Method.GetMethodBody();
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return variableIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override VariableAssignmentElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var value = context.CapturePreceding<ExpressionElement>().Expression;
            var index = variableIndexGetters[instruction.OpCode](instruction.Instruction);
            var type = this.methodBody.LocalVariables[index].LocalType;

            if (type == typeof(bool) && value.Type != typeof(bool))
                value = new BooleanAdapterExpression(value);

            return new VariableAssignmentElement(index, value);
        }
    }
}
