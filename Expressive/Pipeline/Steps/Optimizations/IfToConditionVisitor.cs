using System.Linq.Expressions;
using Expressive.Elements;

namespace Expressive.Pipeline.Steps.Optimizations {
    public class IfToConditionVisitor : ElementVisitor {
        protected override IElement VisitIfThen(IfThenElement ifThen) {
            ifThen = (IfThenElement)base.VisitIfThen(ifThen);
            if (ifThen.Then.Count != 1 || ifThen.Else.Count != 1)
                return ifThen;

            var thenAssignment = ifThen.Then[0] as VariableAssignmentElement;
            var elseAssignment = ifThen.Else[0] as VariableAssignmentElement;

            if (thenAssignment == null || elseAssignment == null || thenAssignment.VariableIndex != elseAssignment.VariableIndex)
                return ifThen;
               
            return new VariableAssignmentElement(thenAssignment.VariableIndex, Expression.Condition(
                ifThen.Condition, thenAssignment.Value, elseAssignment.Value
            ));
        }
    }
}