using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class NewobjToNew : CallToExpression {
        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode == OpCodes.Newobj;
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            var constructor = (ConstructorInfo)((MethodReferenceInstruction)instruction).Method;
            var arguments = this.CaptureArguments(constructor.GetParameters(), constructor, context);

            return Expression.New(constructor, arguments);
        }
    }
}
