using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Expressive.Disassembly.Instructions;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class NewobjToNew : CallToElement {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Newobj;
        }

        public override IElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var constructor = (ConstructorInfo)((MethodReferenceInstruction)instruction.Instruction).Method;
            var arguments = this.CaptureArguments(constructor.GetParameters(), constructor, context);

            return new ExpressionElement(Expression.New(constructor, arguments));
        }
    }
}
