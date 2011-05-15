using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public class BranchToCondition : ElementInterpretation<BranchingElement, IElement> {
        private static readonly IDictionary<string, Func<Expression, Expression>> unary = new Dictionary<string, Func<Expression, Expression>> {
            { OpCodes.Brtrue.Name,  e => e },
            { OpCodes.Brfalse.Name, e => Expression.Not(e) },
        };

        private static readonly IDictionary<string, Func<Expression, Expression, Expression>> binary = new Dictionary<string, Func<Expression, Expression, Expression>> {
            { OpCodes.Beq.Name, Expression.Equal },
            { OpCodes.Bge.Name, Expression.GreaterThanOrEqual },
            { OpCodes.Bgt.Name, Expression.GreaterThan },
            { OpCodes.Ble.Name, Expression.LessThanOrEqual },
            { OpCodes.Blt.Name, Expression.LessThan },
            { OpCodes.Bne_Un.Name.SubstringBefore("."), Expression.NotEqual }
        };

        public override IElement Interpret(BranchingElement branch, IndividualDecompilationContext context) {
            var condition = CaptureCondition(branch, context);
            var targetAsExpression = AsSingleExpression(branch.Target);
            var fallbackAsExpression = AsSingleExpression(branch.Fallback);

            condition = BooleanSupport.ConvertIfRequired(condition, typeof(bool));

            if (targetAsExpression != null && fallbackAsExpression != null) {
                BooleanSupport.ConvertIfRequired(ref targetAsExpression, ref fallbackAsExpression);
                return new ExpressionElement(Expression.Condition(condition, targetAsExpression, fallbackAsExpression));
            }

            var ifTrue = branch.Target;
            var ifFalse = branch.Fallback;
            if (ifTrue.Count == 0) {
                condition = Expression.Not(condition);
                ifFalse = branch.Target;
                ifTrue = branch.Fallback;
            }

            return new IfThenElement(condition, ifTrue, ifFalse);
        }

        private Expression CaptureCondition(BranchingElement branch, IndividualDecompilationContext context) {
            var rootOpCodeName = branch.OpCode.Name.SubstringBefore(".");
            var isUnary = unary.ContainsKey(rootOpCodeName);
            if (isUnary) {
                var operand = context.CapturePreceding<ExpressionElement>().Expression;
                operand = BooleanSupport.ConvertIfRequired(operand, typeof(bool));
                return unary[rootOpCodeName].Invoke(operand);
            }

            var right = context.CapturePreceding<ExpressionElement>().Expression;
            var left = context.CapturePreceding<ExpressionElement>().Expression;
            return binary[rootOpCodeName].Invoke(left, right);
        }

        private Expression AsSingleExpression(IList<IElement> elements) {
            if (elements.Count == 0)
                return null;

            if (elements.Count > 1)
                return null;

            var expressionElement = elements[0] as ExpressionElement;
            return expressionElement != null ? expressionElement.Expression : null;
        }
    }
}
