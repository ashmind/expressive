﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class RetToReturn : ElementInterpretation<InstructionElement, ReturnElement> {
        private Type returnType;

        public override void Initialize(DecompilationContext context) {
            var method = context.Method as MethodInfo;
            this.returnType = method != null ? method.ReturnType : typeof(void);

            base.Initialize(context);
        }

        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode == OpCodes.Ret;
        }

        public override ReturnElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var result = this.returnType != typeof(void)
                       ? context.CapturePreceding<ExpressionElement>().Expression
                       : null;

            result = BooleanSupport.ConvertIfRequired(result, this.returnType);
            return new ReturnElement(result);
        }
    }
}