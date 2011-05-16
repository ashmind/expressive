using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class NewobjToNew : CallToExpression {
        public override bool CanInterpret(InstructionElement element) {
            return element.OpCode == OpCodes.Newobj;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var constructor = (ConstructorInfo)((MethodReferenceInstruction)instruction.Instruction).Method;
            var arguments = this.CaptureArguments(constructor.GetParameters(), constructor, context);

            return new ExpressionElement(Expression.New(constructor, arguments));
        }
    }
}
