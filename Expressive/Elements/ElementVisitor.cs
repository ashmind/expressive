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
            var visited = this.TryVisit<ExpressionElement>(this.TransparentlyVisitExpressionElement, ref element)
                       || this.TryVisit<InstructionElement>(this.VisitInstruction, ref element)
                       || this.TryVisit<VariableAssignmentElement>(this.VisitAssignment, ref element)
                       || this.TryVisit<ReturnElement>(this.VisitReturn, ref element)
                       || this.TryVisit<ConditionalBranchElement>(this.VisitConditionalBranch, ref element)
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

        protected virtual IElement VisitAssignment(VariableAssignmentElement assignment) {
            var value = this.Visit(assignment.Value);
            return value == assignment.Value
                 ? assignment
                 : new VariableAssignmentElement(assignment.VariableIndex, value);
        }

        protected virtual IElement VisitReturn(ReturnElement @return) {
            var result = (@return.Result != null) ? this.Visit(@return.Result) : null;
            return result == @return.Result
                 ? @return
                 : new ReturnElement(result);
        }

        protected virtual IElement VisitConditionalBranch(ConditionalBranchElement branch) {
            this.VisitList(branch.IfTrue);
            this.VisitList(branch.IfFalse);

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
