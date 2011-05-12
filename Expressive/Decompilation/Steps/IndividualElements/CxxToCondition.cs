using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using AshMind.Extensions;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    using BinaryConverter = Func<Expression, Expression, Expression>;

    public class CxxToCondition : ElementInterpretation<InstructionElement, ExpressionElement> {
        private static readonly IDictionary<string, BinaryConverter> conditions = new Dictionary<string, BinaryConverter> {
            { OpCodes.Ceq.Name, Expression.Equal },
            { OpCodes.Cgt.Name, Expression.GreaterThan },
            { OpCodes.Clt.Name, Expression.LessThan }
        };

        public override bool CanInterpret(InstructionElement instruction) {
            return conditions.Keys.Any(k => instruction.OpCode.Name.StartsWith(k));
        }

        public override ExpressionElement Interpret(InstructionElement instruction, IndividualDecompilationContext context) {
            var right = context.CapturePreceding<ExpressionElement>().Expression;
            var left = context.CapturePreceding<ExpressionElement>().Expression;

            BooleanSupport.ConvertIfRequired(ref left, ref right);

            var condition = conditions[instruction.OpCode.Name.SubstringBefore(".")];
            return new ExpressionElement(Expression.Condition(
                condition(left, right),
                Expression.Constant(1),
                Expression.Constant(0)
            ));
        }
    }
}
