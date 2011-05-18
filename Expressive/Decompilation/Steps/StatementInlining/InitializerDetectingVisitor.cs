using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class InitializerDetectingVisitor : ElementVisitor {
        public override void VisitList(IList<IElement> elements) {
            base.VisitList(elements);
            for (var i = elements.Count - 1; i >= 0; i--) {
                var ignore = TryWith<NewExpression>(i, elements, this.CollectObjectInitializer)
                          || TryWith<NewArrayWithSizeExpression>(i, elements, this.CollectArrayInitializer);
            }
        }

        protected virtual bool TryWith<TNewExpression>(
            int index, IList<IElement> elements,
            Func<TNewExpression, int, int, IList<IElement>, Expression> collect
        ) {
            TNewExpression @new;
            VariableAssignmentElement assignment;
            var matched = Matcher
                .For(elements[index]).As<VariableAssignmentElement>()
                .AssignTo(out assignment)
                .For(v => v.Value).As<TNewExpression>()
                    .AssignTo(out @new)
                .Matched;

            if (!matched)
                return false;

            var initializer = collect(@new, assignment.VariableIndex, index, elements);
            if (initializer != null)
                assignment.Value = initializer;

            return true;
        }

        protected virtual NewArrayExpression CollectArrayInitializer(NewArrayWithSizeExpression newArray, int variableIndex, int elementIndex, IList<IElement> elements) {
            var values = new List<Expression>();
            var lastIndex = -1;
            for (var i = elementIndex + 1; i < elements.Count; i++) {
                var value = (Expression)null;
                var matched = Matcher
                    .For(elements[i]).As<ArrayItemAssignmentElement>()
                        .For(
                            e => e.Index,
                            m => m.AsConstant().Value<int>().Is(lastIndex + 1).AssignTo(out lastIndex)
                        )
                        .Do(a => value = a.Value)
                    .For(m => m.Array).As<LocalExpression>()
                        .Match(l => l.Index == variableIndex)
                    .Matched;

                if (!matched)
                    break;

                values.Add(value);
                elements.RemoveAt(i);
                i -= 1;
            }

            if (values.Count == 0)
                return null;

            return Expression.NewArrayInit(newArray.Type.GetElementType(), values);
        }

        protected virtual MemberInitExpression CollectObjectInitializer(NewExpression @new, int variableIndex, int elementIndex, IList<IElement> elements) {
            var bindings = new List<MemberBinding>();
            for (var i = elementIndex + 1; i < elements.Count; i++) {
                var binding = (MemberBinding)null;
                var matched = Matcher
                    .For(elements[i]).As<MemberAssignmentElement>()
                        .Do(a => binding = Expression.Bind(a.Member, a.Value))
                    .For(m => m.Instance).As<LocalExpression>()
                        .Match(l => l.Index == variableIndex)
                    .Matched;

                if (!matched)
                    break;

                bindings.Add(binding);
                elements.RemoveAt(i);
                i -= 1;
            }

            if (bindings.Count == 0)
                return null;

            return Expression.MemberInit(@new, bindings);
        }
    }
}
