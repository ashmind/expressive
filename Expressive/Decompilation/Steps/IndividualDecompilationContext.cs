using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;
using Expressive.Elements.Presentation;

namespace Expressive.Decompilation.Steps {
    public class IndividualDecompilationContext {
        private readonly IList<IElement> elements;
        private readonly Stack<BranchStackFrame> branchStack;

        public IndividualDecompilationContext(IList<IElement> elements, Stack<BranchStackFrame> branchStack) {
            this.elements = elements;
            this.branchStack = branchStack;
        }

        public Expression CapturePreceding() {
            var precedingIndex = this.GetPrecedingIndex();
            var preceding = this.elements[precedingIndex];

            this.elements.RemoveAt(precedingIndex);
            this.CurrentIndex -= 1;

            return ToExpression(preceding);
        }

        public Expression GetPreceding() {
            var preceding = this.elements[this.GetPrecedingIndex()];
            return ToExpression(preceding);
        }

        private int GetPrecedingIndex() {
            if (this.CurrentIndex == 0) {
                this.ImportPrecedingIntoBranches(this.branchStack.GetEnumerator());
                this.CurrentIndex += 1;
            }

            return this.CurrentIndex - 1;
        }

        private void ImportPrecedingIntoBranches(IEnumerator<BranchStackFrame> enumerator) {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("There is no previous element in this context.");

            var frame = enumerator.Current;
            var targetIndex = frame.CurrentIndex - frame.Branching.ParameterCount;
            if (targetIndex < 0) {
                ImportPrecedingIntoBranches(enumerator);
                frame.CurrentIndex += 1;
            }

            targetIndex = frame.CurrentIndex - frame.Branching.ParameterCount;
            if (targetIndex < 0)
                throw new InvalidOperationException("There are no available elements before " + frame.Branching + ".");

            foreach (var branch in frame.Branching.GetBranches()) {
                branch.Insert(0, frame.Elements[targetIndex]);
            }
            frame.Elements.RemoveAt(targetIndex);
            frame.CurrentIndex -= 1;
        }

        private static Expression ToExpression(IElement element) {
            var typed = element as ExpressionElement;
            if (typed == null)
                throw new InvalidOperationException("Element " + element + " must be an ExpressionElement to be used in this context.");
            return typed.Expression;
        }

        public void VerifyPrecedingCount(int requiredCount, Func<int, string, string> getMessage) {
            if (this.CurrentIndex >= requiredCount)
                return;

            throw new InvalidOperationException(getMessage(
                this.CurrentIndex,
                ElementHelper.ToString(elements.Take(this.CurrentIndex), Indent.FourSpaces)
            ));
        }

        public int CurrentIndex { get; set; }
    }
}