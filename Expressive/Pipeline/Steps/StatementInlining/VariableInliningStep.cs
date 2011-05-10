using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AshMind.Extensions;
using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Pipeline.Steps.StatementInlining {
    public class VariableInliningStep : IInterpretationStep {
        #region EsimatingVisitor Class

        private class EstimatingVisitor : ElementVisitor {
            private readonly IDictionary<int, int> assignmentCount = new Dictionary<int, int>();

            private EstimatingVisitor() {
            }

            public static HashSet<int> Estimate(IList<IElement> elements) {
                var visitor = new EstimatingVisitor();
                visitor.VisitList(elements);
                return visitor.assignmentCount.Where(p => p.Value == 1)
                                              .Select(p => p.Key)
                                              .ToSet();
            }

            protected override IElement VisitAssignment(VariableAssignmentElement assignment) {
                assignmentCount[assignment.VariableIndex] =
                    assignmentCount.GetValueOrDefault(assignment.VariableIndex) + 1;
                return base.VisitAssignment(assignment);
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

            protected override IElement VisitAssignment(VariableAssignmentElement assignment) {
                assignment = (VariableAssignmentElement)base.VisitAssignment(assignment);
                if (!inlineable.Contains(assignment.VariableIndex))
                    return assignment;
                
                values.Add(assignment.VariableIndex, assignment.Value);
                return null;
            }
        }

        #endregion
        
        public void Apply(IList<IElement> elements, InterpretationContext context) {
            var inlineable = EstimatingVisitor.Estimate(elements);
            InliningVisitor.Inline(elements, inlineable);
        }
    }
}
