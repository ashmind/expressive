using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements.Expressions;

namespace Expressive.Elements {
    public abstract class ElementVisitor : ExpressionTreeVisitor {
        public virtual void VisitList(IList<IElement> elements) {
            for (var i = 0; i < elements.Count; i++) {
                var result = this.Visit(elements[i]);
                if (result != null) {
                    elements[i] = result;
                }
                else {
                    elements.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        public virtual IElement Visit(IElement element) {
            if (element == null)
                return null;

            var visited = this.TryVisit<ExpressionElement>(this.TransparentlyVisitExpressionElement, ref element)
                       || this.TryVisit<InstructionElement>(this.VisitInstruction, ref element)
                       || this.TryVisit<VariableAssignmentElement>(this.VisitVariableAssignment, ref element)
                       || this.TryVisit<MemberAssignmentElement>(this.VisitMemberAssignment, ref element)
                       || this.TryVisit<ArrayItemAssignmentElement>(this.VisitArrayItemAssignment, ref element)
                       || this.TryVisit<ReturnElement>(this.VisitReturn, ref element)
                       || this.TryVisit<BranchingElement>(this.VisitBranching, ref element)
                       || this.TryVisit<IfThenElement>(this.VisitIfThen, ref element);

            if (!visited)
                throw new NotSupportedException("Element type " + element.GetType() + " is not supported.");

            return element;
        }

        private bool TryVisit<TElement>(Func<TElement, IElement> visit, ref IElement element) 
            where TElement : class, IElement
        {
            var cast = element as TElement;
            if (cast == null)
                return false;

            element = visit(cast);
            return true;
        }

        private IElement TransparentlyVisitExpressionElement(ExpressionElement expression) {
            expression.Expression = this.Visit(expression.Expression);
            return expression;
        }

        protected virtual IElement VisitInstruction(InstructionElement instruction) {
            return instruction;
        }

        protected virtual IElement VisitVariableAssignment(VariableAssignmentElement assignment) {
            assignment.Value = this.Visit(assignment.Value);
            return assignment;
        }

        protected virtual IElement VisitMemberAssignment(MemberAssignmentElement assignment) {
            assignment.Instance = this.Visit(assignment.Instance);
            assignment.Value = this.Visit(assignment.Value);
            return assignment;
        }

        protected virtual IElement VisitArrayItemAssignment(ArrayItemAssignmentElement assignment) {
            assignment.Array = this.Visit(assignment.Array);
            assignment.Index = this.Visit(assignment.Index);
            assignment.Value = this.Visit(assignment.Value);
            return assignment;
        }

        protected virtual IElement VisitReturn(ReturnElement @return) {
            @return.Result = this.Visit(@return.Result);
            return @return;
        }

        protected virtual IElement VisitBranching(BranchingElement branch) {
            this.VisitList(branch.Target);
            this.VisitList(branch.Fallback);

            return branch;
        }

        protected virtual IElement VisitIfThen(IfThenElement ifThen) {
            ifThen.Condition = this.Visit(ifThen.Condition);
            this.VisitList(ifThen.Then);
            this.VisitList(ifThen.Else);

            return ifThen;
        }
    }
}
