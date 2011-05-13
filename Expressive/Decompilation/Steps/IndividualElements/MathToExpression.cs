using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    using BinaryConverter = Func<Expression, Expression, Expression>;

    public class MathToExpression : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<OpCode, BinaryConverter> operators = new Dictionary<OpCode, BinaryConverter> {
            { OpCodes.Add, Expression.Add },
            { OpCodes.And, Expression.And },
            { OpCodes.Or,  Expression.Or },
            { OpCodes.Sub, Expression.Subtract }
        };

        public override bool CanInterpret(InstructionElement instruction) {
            return operators.ContainsKey(instruction.OpCode);
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var right = context.CapturePreceding<ExpressionElement>().Expression;
            var left = context.CapturePreceding<ExpressionElement>().Expression;
            var binary = operators[instruction.OpCode];

            return new ExpressionElement(binary(left, right));
        }
    }
}
