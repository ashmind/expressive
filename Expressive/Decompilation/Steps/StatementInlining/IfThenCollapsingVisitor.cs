using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class IfThenCollapsingVisitor : ElementVisitor {
         protected override IElement VisitIfThen(IfThenElement ifThen) {
            ifThen = (IfThenElement)base.VisitIfThen(ifThen);
            if (ifThen.Else.Count > 0)
                return ifThen;

            if (ifThen.Then.Count > 1)
                return ifThen;

            var thenAsIf = ifThen.Then[0] as IfThenElement;
            if (thenAsIf == null)
                return ifThen;

            return new IfThenElement(
                Expression.AndAlso(ifThen.Condition, thenAsIf.Condition),
                thenAsIf.Then, thenAsIf.Else
            );
        }
    }
}
