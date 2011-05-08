using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class LdargToParameterStep : BranchingAwareStepBase {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> parameterIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldarg_0, _ => 0 },
            { OpCodes.Ldarg_1, _ => 1 },
            { OpCodes.Ldarg_2, _ => 2 },
            { OpCodes.Ldarg_3, _ => 3 },
            { OpCodes.Ldarg_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldarg,   x => ((InlineVarInstruction)x).Ordinal }
        };

        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            var alreadyExtracted = context.ExtractedParameters.ToDictionary(p => p.Name);
            var parameters = context.Method.GetParameters()
                                           .Select(p => alreadyExtracted.GetValueOrDefault(p.Name) ?? Expression.Parameter(p.ParameterType, p.Name))
                                           .ToList();

            if (!context.Method.IsStatic)
                parameters.Insert(0, alreadyExtracted.GetValueOrDefault("<this>") ?? Expression.Parameter(context.Method.DeclaringType, "<this>"));

            for (var i = 0; i < elements.Count; i++) {
                var instruction = elements[i] as InstructionElement;
                if (instruction == null)
                    continue;

                var indexGetter = parameterIndexGetters.GetValueOrDefault(instruction.OpCode);
                if (indexGetter == null)
                    continue;

                var parameter = parameters[indexGetter(instruction.Instruction)];
                elements[i] = new ExpressionElement(parameter);
                context.ExtractedParameters.Add(parameter);
            }
        }
    }
}
