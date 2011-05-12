﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdnullToConstant : ElementInterpretation<InstructionElement, ExpressionElement> {
        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ldnull;
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            return new ExpressionElement(Expression.Constant(null));
        }
    }
}
