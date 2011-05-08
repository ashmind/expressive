using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class LdstrToConstantStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                var instruction = elements[i] as InstructionElement;
                if (instruction == null || instruction.OpCode != OpCodes.Ldstr)
                    continue;

                var value = ((InlineStringInstruction)instruction.Instruction).String;
                elements[i] = new ExpressionElement(Expression.Constant(value));
            }
        }
    }
}
