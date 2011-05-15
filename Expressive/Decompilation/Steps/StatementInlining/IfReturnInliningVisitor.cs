using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class IfReturnInliningVisitor : ElementVisitor {
        public override void VisitList(IList<IElement> elements) {
            base.VisitList(elements);
            for (var i = elements.Count - 1; i >= 0; i--) {
                var ifThen = elements[i] as IfThenElement;
                if (ifThen == null)
                    continue;

                if (TryExtractElseIfThenEndsWithReturn(ifThen, elements, i)) {
                    i = elements.Count;
                    continue;
                }

                TryInlineIfThenReturnFollowedByReturn(ifThen, elements, i);
            }
        }

        private bool TryExtractElseIfThenEndsWithReturn(IfThenElement ifThen, IList<IElement> elements, int index) {
            if (ifThen.Else.Count == 0 || !(ifThen.Then.Last() is ReturnElement))
                return false;
            
            elements.InsertRange(index + 1, ifThen.Else);
            ifThen.Else.Clear();
            return true;
        }

        private static void TryInlineIfThenReturnFollowedByReturn(IfThenElement ifThen, IList<IElement> elements, int index) {
            if (index == elements.Count - 1)
                return;

            if (ifThen.Then.Count != 1 || ifThen.Else.Count > 0)
                return;

            var returnInIf = ifThen.Then.Single() as ReturnElement;
            var returnAfterIf = elements[index + 1] as ReturnElement;
            if (returnInIf == null || returnAfterIf == null)
                return;

            if (returnInIf.Result == null || returnAfterIf.Result == null)
                return;

            elements[index] = new ReturnElement(Expression.Condition(
                ifThen.Condition,
                returnInIf.Result,
                returnAfterIf.Result
            ));

            elements.RemoveAt(index + 1);
        }
    }
}
