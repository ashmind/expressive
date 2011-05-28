using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;
using Expressive.Disassembly.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    using BinaryOperator = Func<Expression, Expression, Expression>;
    using UnaryOperator = Func<Expression, Expression>;

    public class MathToExpression : InstructionToExpression {
        private static readonly IDictionary<OpCode, BinaryOperator> binaryOperators = new Dictionary<OpCode, BinaryOperator> {
            { OpCodes.Add, Expression.Add },
            { OpCodes.And, Expression.And },
            { OpCodes.Div, Expression.Divide },
            { OpCodes.Mul, Expression.Multiply },
            { OpCodes.Or,  Expression.Or },
            { OpCodes.Rem, Expression.Modulo },
            { OpCodes.Shl, Expression.LeftShift },
            { OpCodes.Shr, Expression.RightShift },
            { OpCodes.Sub, Expression.Subtract },
            { OpCodes.Xor, Expression.ExclusiveOr }
        };

        private static readonly IDictionary<OpCode, UnaryOperator> unaryOperators = new Dictionary<OpCode, UnaryOperator> {
            { OpCodes.Not, Expression.Not },
            { OpCodes.Neg, Expression.Negate } 
        };

        public override bool CanInterpret(Instruction instruction) {
            return binaryOperators.ContainsKey(instruction.OpCode)
                || unaryOperators.ContainsKey(instruction.OpCode);
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            var singleOrRight = context.CapturePreceding();
            var unary = unaryOperators.GetValueOrDefault(instruction.OpCode);
            if (unary != null)
                return unary(singleOrRight);

            var left = context.CapturePreceding();
            var binary = binaryOperators[instruction.OpCode];

            Adapt(left, ref singleOrRight);

            return binary(left, singleOrRight);
        }

        private void Adapt(Expression left, ref Expression right) {
            if (left.Type == right.Type)
                return;

            right = Expression.Convert(right, left.Type);
        }
    }
}
