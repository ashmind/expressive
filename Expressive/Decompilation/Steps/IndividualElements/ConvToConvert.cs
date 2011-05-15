using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class ConvToConvert : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, Func<Expression, Expression>> conversions = new Dictionary<OpCode, Func<Expression, Expression>> {
            { OpCodes.Conv_I2, e => Expression.Convert(e, typeof(short)) },
            { OpCodes.Conv_I8, e => Expression.Convert(e, typeof(long)) }
        };

        public override bool CanInterpret(InstructionElement instruction) {
            return conversions.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var target = context.CapturePreceding<ExpressionElement>().Expression;
            return new ExpressionElement(conversions[instruction.OpCode](target));
        }
    }
}
