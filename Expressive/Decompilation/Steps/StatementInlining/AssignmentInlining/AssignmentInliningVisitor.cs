using System.Collections.Generic;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Decompilation.Steps.StatementInlining.AssignmentInlining {
    internal class AssignmentInliningVisitor : ElementVisitor {
        private readonly HashSet<int> inlineable;
        private readonly IDictionary<int, Expression> values = new Dictionary<int, Expression>();

        private AssignmentInliningVisitor(HashSet<int> inlineable) {
            this.inlineable = inlineable;
        }

        public static void Inline(IList<IElement> elements, HashSet<int> inlineable) {
            var visitor = new AssignmentInliningVisitor(inlineable);
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
}