using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Decompilation.Steps.StatementInlining.AssignmentInlining {
    public class AssignmentEstimatingVisitor : ElementVisitor {
        private class VariableDetails {
            public VariableDetails() {
                this.Trivial = true;
            }

            public int AssignmentCount { get; set; }
            public int UseCount { get; set; }
            public bool Trivial { get; set; }
        }

        private readonly Func<int, bool> variableIndexPredicate;
        private readonly IDictionary<int, VariableDetails> details = new Dictionary<int, VariableDetails>();

        private AssignmentEstimatingVisitor(Func<int, bool> variableIndexPredicate) {
            this.variableIndexPredicate = variableIndexPredicate;
        }

        public static HashSet<int> Estimate(IList<IElement> elements, Func<int, bool> variableIndexPredicate) {
            var visitor = new AssignmentEstimatingVisitor(variableIndexPredicate);
            visitor.VisitList(elements);
            return visitor.details.Where(p => p.Value.AssignmentCount == 1 && (p.Value.UseCount < 2 || p.Value.Trivial))
                                  .Select(p => p.Key)
                                  .ToSet();
        }

        protected override IElement VisitVariableAssignment(VariableAssignmentElement assignment) {
            if (variableIndexPredicate(assignment.VariableIndex)) {
                var details = GetDetails(assignment.VariableIndex);
                details.AssignmentCount += 1;
                details.Trivial = details.Trivial && IsTrivial(assignment.Value);
            }
            return base.VisitVariableAssignment(assignment);
        }

        protected override Expression VisitLocal(LocalExpression local) {
            if (variableIndexPredicate(local.Index)) {
                GetDetails(local.Index).UseCount += 1;
            }

            return base.VisitLocal(local);
        }

        private VariableDetails GetDetails(int localIndex) {
            var detailsItem = this.details.GetValueOrDefault(localIndex);
            if (detailsItem == null) {
                detailsItem = new VariableDetails();
                this.details.Add(localIndex, detailsItem);
            }

            return detailsItem;
        }

        private bool IsTrivial(Expression expression) {
            return expression is ParameterExpression
                || (
                    expression is ConstantExpression
                    && expression.Type.IsPrimitive
                );
        }
    }
}