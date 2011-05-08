using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;
using Expressive.Common;
using Expressive.Elements;
using Expressive.Elements.Presentation;

namespace Expressive.Pipeline.Steps {
    public class JumpToExpressionStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                var jump = elements[i] as ConditionalJumpElement;
                if (jump == null)
                    continue;

                var preceding = new PartialList<IElement>(elements, 0, i);
                context.ApplyPipeline(preceding);
                i = preceding.Count;

                var condition = ToExpression(elements[i - 1]);

                context.ApplyPipeline(jump.FollowingBranch);
                var following = ToSingleExpression(jump.FollowingBranch);

                context.ApplyPipeline(jump.TargetBranch);
                var target = ToSingleExpression(jump.TargetBranch);

                elements[i] = new ExpressionElement(Expression.Condition(
                    condition,
                    jump.JumpWhenTrue ? target : following,
                    jump.JumpWhenTrue ? following : target
                ));
                elements.RemoveAt(i - 1); // remove condition
                i -= 1;
            }
        }

        private Expression ToSingleExpression(IList<IElement> elements) {
            if (elements.Count == 0)
                return null;

            if (elements.Count > 1)
                throw new InvalidOperationException("Cannot convert to single expression: " + Environment.NewLine + ElementHelper.ToString(elements, Indent.FourSpaces));

            return ToExpression(elements[0]);
        }

        private Expression ToExpression(IElement element) {
            var expressionElement = element as ExpressionElement;
            if (expressionElement == null)
                throw new InvalidOperationException("Element " + element + " must be an ExpressionElement to be used in condition.");

            return expressionElement.Expression;
        }
    }
}
