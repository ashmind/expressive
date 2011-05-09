using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive.Pipeline.Steps.IndividualElements {
    public class BranchToCondition : ElementInterpretation<ConditionalBranchElement, IElement> {
        public override IElement Interpret(ConditionalBranchElement branch, IndividualInterpretationContext context) {
            var condition = context.CapturePreceding<ExpressionElement>(-1).Expression;
            var ifTrueAsExpression = AsSingleExpression(branch.IfTrue);
            var ifFalseAsExpression = AsSingleExpression(branch.IfFalse);

            if (condition.Type != typeof(bool))
                condition = new BooleanAdapterExpression(condition);

            if (ifTrueAsExpression != null && ifFalseAsExpression != null)
                return new ExpressionElement(Expression.Condition(condition, ifTrueAsExpression, ifFalseAsExpression));

            var ifTrue = branch.IfTrue;
            var ifFalse = branch.IfFalse;
            if (ifTrue.Count == 0) {
                condition = Expression.Not(condition);
                ifFalse = branch.IfTrue;
                ifTrue = branch.IfFalse;
            }

            return new IfThenElement(condition, ifTrue, ifFalse);
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
