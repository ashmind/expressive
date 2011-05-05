using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class LdstrToConstantStep : IInterpretationStep {
        public void Apply(InterpretationWorkspace workspace) {
            for (var i = 0; i < workspace.Elements.Count; i++) {
                var instruction = workspace.Elements[i] as InstructionElement;
                if (instruction == null || instruction.OpCode != OpCodes.Ldstr)
                    continue;

                var value = ((InlineStringInstruction)instruction.Instruction).String;
                workspace.Elements[i] = new ExpressionElement(Expression.Constant(value));
            }
        }
    }
}
