using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class LdcToConstant : InstructionToExpression {
        private static readonly string LdcNamePrefix = OpCodes.Ldc_I4.Name.SubstringBefore(".");

        public override bool CanInterpret(Instruction instruction) {
            return instruction.OpCode.Name.StartsWith(LdcNamePrefix);
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            return Expression.Constant(GetValue(instruction));
        }

        private object GetValue(Instruction instruction) {
            var valueInstruction = instruction as IValueInstruction;
            if (valueInstruction != null)
                return valueInstruction.Value;
                
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
    }
}
