using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class VariableInliningStep : IDecompilationStep {
        #region EstimatingVisitor Class

        private class EstimatingVisitor : ElementVisitor {
            private class VariableDetails {
                public VariableDetails() {
                    this.Trivial = true;
                }

                public int AssignmentCount { get; set; }
                public int UseCount { get; set; }
                public bool Trivial { get; set; }
            }

            private readonly IDictionary<int, VariableDetails> details = new Dictionary<int, VariableDetails>();

            private EstimatingVisitor() {
            }

            public static HashSet<int> Estimate(IList<IElement> elements) {
                var visitor = new EstimatingVisitor();
                visitor.VisitList(elements);
                return visitor.details.Where(p => p.Value.AssignmentCount == 1 && (p.Value.UseCount < 2 || p.Value.Trivial))
                                      .Select(p => p.Key)
                                      .ToSet();
            }

            protected override IElement VisitVariableAssignment(VariableAssignmentElement assignment) {
                var details = GetDetails(assignment.VariableIndex);
                details.AssignmentCount += 1;
                details.Trivial = details.Trivial && IsTrivial(assignment.Value);
                return base.VisitVariableAssignment(assignment);
            }

            protected override Expression VisitLocal(LocalExpression local) {
                GetDetails(local.Index).UseCount += 1;
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

        #endregion

        #region InliningVisitor Class

        private class InliningVisitor : ElementVisitor {
            private readonly HashSet<int> inlineable;
            private readonly IDictionary<int, Expression> values = new Dictionary<int, Expression>();

            private InliningVisitor(HashSet<int> inlineable) {
                this.inlineable = inlineable;
            }

            public static void Inline(IList<IElement> elements, HashSet<int> inlineable) {
                var visitor = new InliningVisitor(inlineable);
                visitor.VisitList(elements);
            }

            protected override Expression VisitLocal(LocalExpression local) {
                var value = this.values.GetValueOrDefault(local.Index);
                return value ?? local;
            }

            protected override IElement VisitVariableAssignment(VariableAssignmentElement assignment) {
                assignment = (VariableAssignmentElement)base.VisitVariableAssignment(assignment);
                if (!inlineable.Contains(assignment.VariableIndex))
                    return assignment;
                
                values.Add(assignment.VariableIndex, assignment.Value);
                return null;
            }
        }

        #endregion
        
        public void Apply(IList<IElement> elements, DecompilationContext context) {
            var inlineable = EstimatingVisitor.Estimate(elements);
            InliningVisitor.Inline(elements, inlineable);
        }
    }
}
