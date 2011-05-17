using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using AshMind.Extensions;

using Expressive.Elements.Instructions;

namespace Expressive.Decompilation.Steps.IndividualElements {
    using BinaryConverter = Func<Expression, Expression, Expression>;

    public class CxxToCondition : InstructionToExpression {
        private static readonly IDictionary<string, BinaryConverter> conditions = new Dictionary<string, BinaryConverter> {
            { OpCodes.Ceq.Name, Expression.Equal },
            { OpCodes.Cgt.Name, Expression.GreaterThan },
            { OpCodes.Clt.Name, Expression.LessThan }
        };

        public override bool CanInterpret(Instruction instruction) {
            return conditions.Keys.Any(k => instruction.OpCode.Name.StartsWith(k));
        }

        public override Expression Interpret(Instruction instruction, IndividualDecompilationContext context) {
            var right = context.CapturePreceding();
            var left = context.CapturePreceding();

            BooleanSupport.ConvertIfRequired(ref left, ref right);

            var condition = conditions[instruction.OpCode.Name.SubstringBefore(".")];
            return Expression.Condition(
                condition(left, right),
                Expression.Constant(1),
                Expression.Constant(0)
            );
        }
    }
}
