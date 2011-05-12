using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdargToParameter : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> parameterIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldarg_0, _ => 0 },
            { OpCodes.Ldarg_1, _ => 1 },
            { OpCodes.Ldarg_2, _ => 2 },
            { OpCodes.Ldarg_3, _ => 3 },
            { OpCodes.Ldarg_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldarg,   x => ((InlineVarInstruction)x).Ordinal }
        };

        private DecompilationContext primaryContext;

        public override void Initialize(DecompilationContext context) {
            this.primaryContext = context;
            context.ExtractedParameters.Clear();

            if (!context.Method.IsStatic)
                context.ExtractedParameters.Add(Expression.Parameter(context.Method.DeclaringType, "<this>"));

            context.ExtractedParameters.AddRange(
                context.Method.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name))
            );
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return parameterIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var indexGetter = parameterIndexGetters[instruction.OpCode];
            var parameter = this.primaryContext.ExtractedParameters[indexGetter(instruction.Instruction)];
            
            return new ExpressionElement(parameter);
        }
    }
}
