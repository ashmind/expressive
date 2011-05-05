using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class LdargToParameterStep : IInterpretationStep {
        private static readonly IDictionary<OpCode, Func<ILInstruction, int>> parameterIndexGetters = new Dictionary<OpCode, Func<ILInstruction, int>> {
            { OpCodes.Ldarg_0, _ => 0 },
            { OpCodes.Ldarg_1, _ => 1 },
            { OpCodes.Ldarg_2, _ => 2 },
            { OpCodes.Ldarg_3, _ => 3 },
            { OpCodes.Ldarg_S, x => ((ShortInlineVarInstruction)x).Ordinal },
            { OpCodes.Ldarg,   x => ((InlineVarInstruction)x).Ordinal }
        };
        
        public void Apply(InterpretationWorkspace workspace) {
            var parameters = workspace.Method.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToList();
            if (!workspace.Method.IsStatic)
                parameters.Insert(0, Expression.Parameter(workspace.Method.DeclaringType, "<this>"));

            for (var i = 0; i < workspace.Elements.Count; i++) {
                var instruction = workspace.Elements[i] as InstructionElement;
                if (instruction == null)
                    continue;

                var indexGetter = parameterIndexGetters.GetValueOrDefault(instruction.OpCode);
                if (indexGetter == null)
                    continue;

                var parameter = parameters[indexGetter(instruction.Instruction)];
                workspace.Elements[i] = new ExpressionElement(parameter);
                workspace.ExtractedParameters.Add(parameter);
            }
        }
    }
}
