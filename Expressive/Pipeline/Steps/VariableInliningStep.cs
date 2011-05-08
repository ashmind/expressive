using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Pipeline.Steps {
    public class VariableInliningStep : BranchingAwareStepBase {
        #region InliningVisitor Class

        private class InliningVisitor : ElementVisitor {
            private readonly VariableAssignmentElement assignment;

            public InliningVisitor(VariableAssignmentElement assignment) {
                this.assignment = assignment;
            }

            public void Inline(IList<IElement> elements) {
                for (var i = 0; i < elements.Count; i++) {
                    elements[i] = this.Visit(elements[i]);
                }
            }

            protected override Expression VisitLocal(LocalExpression local) {
                if (local.Index == this.assignment.VariableIndex) {
                    var expressionElement = this.assignment.Value as ExpressionElement;
                    if (expressionElement == null)
                        throw new InvalidOperationException("Cannot inline element " + this.assignment.Value + " which is not an ExpressionElement.");

                    return expressionElement.Expression;
                }

                return base.VisitLocal(local);
            }

            protected override IElement VisitAssignment(VariableAssignmentElement otherAssignment) {
                if (otherAssignment.VariableIndex == this.assignment.VariableIndex && otherAssignment != assignment)
                    throw new NotSupportedException("Reassignment of local variable " + this.assignment.VariableIndex + " is not supported.");

                return base.VisitAssignment(otherAssignment);
            }
        }

        #endregion

        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            var variablesTried = new HashSet<int>();

            for (var i = 0; i < elements.Count; i++) {
                var assignment = elements[i] as VariableAssignmentElement;
                if (assignment == null || variablesTried.Contains(assignment.VariableIndex))
                    continue;

                this.Inline(assignment, elements);
                i -= 1; // inlining removes assignment

                variablesTried.Add(assignment.VariableIndex);
            }
        }

        private void Inline(VariableAssignmentElement assignment, IList<IElement> elements) {
            new InliningVisitor(assignment).Inline(elements);
            elements.Remove(assignment);
        }
    }
}
