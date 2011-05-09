using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using AshMind.Extensions;
using ClrTest.Reflection;

using Expressive.Elements;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class LdargToParameter : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> parameterIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldarg_0, _ => 0 },
            { OpCodes.Ldarg_1, _ => 1 },
            { OpCodes.Ldarg_2, _ => 2 },
            { OpCodes.Ldarg_3, _ => 3 },
            { OpCodes.Ldarg_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldarg,   x => ((InlineVarInstruction)x).Ordinal }
        };

        private InterpretationContext primaryContext;
        private List<ParameterExpression> parameters;

        public override void Initialize(InterpretationContext context) {
            this.primaryContext = context;
            var alreadyExtracted = context.ExtractedParameters.ToDictionary(p => p.Name);
            this.parameters = context.Method.GetParameters()
                                           .Select(p => alreadyExtracted.GetValueOrDefault(p.Name) ?? Expression.Parameter(p.ParameterType, p.Name))
                                           .ToList();

            if (!context.Method.IsStatic)
                this.parameters.Insert(0, alreadyExtracted.GetValueOrDefault("<this>") ?? Expression.Parameter(context.Method.DeclaringType, "<this>"));
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return parameterIndexGetters.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            var indexGetter = parameterIndexGetters[instruction.OpCode];
            var parameter = parameters[indexGetter(instruction.Instruction)];

            this.primaryContext.ExtractedParameters.Add(parameter);
            return new ExpressionElement(parameter);
        }
    }
}
