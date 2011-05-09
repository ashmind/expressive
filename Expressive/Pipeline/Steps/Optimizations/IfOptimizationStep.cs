using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Elements;

namespace Expressive.Pipeline.Steps.Optimizations {
    public class IfOptimizationStep : IInterpretationStep {
        private class IfToConditionVisitor : ElementVisitor {
            public void Process(IList<IElement> elements) {
                this.VisitAll(elements);
            }

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

        public void Apply(IList<IElement> elements, InterpretationContext context) {
            new IfToConditionVisitor().Process(elements);
        }
    }
}
