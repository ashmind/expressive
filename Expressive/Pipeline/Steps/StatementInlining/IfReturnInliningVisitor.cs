using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Elements;

namespace Expressive.Pipeline.Steps.StatementInlining {
    public class IfReturnInliningVisitor : ElementVisitor {
        public override void VisitList(IList<IElement> elements) {
            base.VisitList(elements);
            for (var i = 0; i < elements.Count; i++) {
                if (i == elements.Count - 1)
                    continue;
                
                var ifThen = elements[i] as IfThenElement;
                if (ifThen == null)
                    continue;

                if (ifThen.Then.Count != 1 || ifThen.Else.Count > 0)
                    continue;

                var returnInIf = ifThen.Then.Single() as ReturnElement;
                var returnAfterIf = elements[i + 1] as ReturnElement;

                if (returnInIf == null || returnAfterIf == null)
                    continue;

                if (returnInIf.Result == null || returnAfterIf.Result == null)
                    continue;

                elements[i] = new ReturnElement(Expression.Condition(
                    ifThen.Condition,
                    returnInIf.Result,
                    returnAfterIf.Result
                ));
                elements.RemoveAt(i + 1);
            }
        }
    }
}
