using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdargToParameter : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<Instruction, int>> parameterIndexGetters = new Dictionary<OpCode, Func<Instruction, int>> {
            { OpCodes.Ldarg_0,  _ => 0 },
            { OpCodes.Ldarg_1,  _ => 1 },
            { OpCodes.Ldarg_2,  _ => 2 },
            { OpCodes.Ldarg_3,  _ => 3 },
            { OpCodes.Ldarg,    x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Ldarg_S,  x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Ldarga,   x => ((VariableReferenceInstruction)x).Ordinal },
            { OpCodes.Ldarga_S, x => ((VariableReferenceInstruction)x).Ordinal },
        };

        private DecompilationContext primaryContext;

        public override void Initialize(DecompilationContext context) {
            this.primaryContext = context;
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return parameterIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var indexGetter = parameterIndexGetters[instruction.OpCode];
            var parameter = this.primaryContext.GetParameter(indexGetter(instruction.Instruction));
            
            return new ExpressionElement(parameter);
        }
    }
}
