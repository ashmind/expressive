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
    public class LdcToConstant : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly string LdcNamePrefix = OpCodes.Ldc_I4.Name.SubstringBefore(".");

        public override bool CanInterpret(InstructionElement instruction) {
            return instruction.OpCode.Name.StartsWith(LdcNamePrefix);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualInterpretationContext context) {
            return new ExpressionElement(Expression.Constant(
                GetValue(instruction.Instruction)
            ));
        }

        private object GetValue(ILInstruction instruction) {
            var value = this.GetValueOrNull<ShortInlineIInstruction>(instruction, i => i.Byte)
                     ?? this.GetValueOrNull<InlineIInstruction>(instruction, i => i.Int32)
                     ?? this.GetValueOrNull<InlineI8Instruction>(instruction, i => i.Int64)
                     ?? this.GetValueOrNull<ShortInlineRInstruction>(instruction, i => i.Single)
                     ?? this.GetValueOrNull<InlineRInstruction>(instruction, i => i.Double);

            if (value != null)
                return value;
                
            var parts = instruction.OpCode.Name.Split('.');
            if (parts.Length < 3)
                throw new InvalidOperationException("Cannot extract value from " + instruction.OpCode + ".");

            if (!parts[1].Equals("i4", StringComparison.InvariantCultureIgnoreCase))
                throw new NotSupportedException("Cannot extract value from " + instruction.OpCode + ": " + parts[1] + " is not yet supported here.");

            var valueString = parts[2];
            if (valueString.Equals("M1", StringComparison.InvariantCultureIgnoreCase))
                return -1;

            return int.Parse(valueString);
        }

        private object GetValueOrNull<TInstruction>(ILInstruction instruction, Func<TInstruction, object> getValue) 
            where TInstruction : ILInstruction
        {
            var typed = instruction as TInstruction;
            if (typed == null)
                return null;

            return getValue(typed);
        }
    }
}
