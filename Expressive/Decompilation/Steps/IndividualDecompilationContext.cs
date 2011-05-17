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
            //BranchStackFrame frame;
            //IEnumerator<BranchStackFrame> stackEnumerator;

            //if (branchStack.Count == 0) {
                var frame = new BranchStackFrame(this.elements, null, this.CurrentIndex);
                var stackEnumerator = this.branchStack.GetEnumerator();
            //}
            //else {
            //    stackEnumerator = this.branchStack.GetEnumerator();
            //    stackEnumerator.MoveNext();
            //    frame = stackEnumerator.Current;
            //}

            var index = this.FindPrecedingIndex(this.CurrentIndex - 1, frame, stackEnumerator);
            this.CurrentIndex = frame.CurrentIndex;
            return index;
        }

        private int FindPrecedingIndex(int startingIndex, BranchStackFrame frame, IEnumerator<BranchStackFrame> stack) {
            var index = startingIndex;
            while (true) {
                if (index < 0) {
                    this.ImportPrecedingIntoBranches(stack);
                    frame.CurrentIndex += 1;
                    index += 1;
                }

                var kind = frame.Elements[index].Kind;
                if (kind == ElementKind.Undefined)
                    throw new InvalidOperationException("Element " + frame.Elements[index] + " must be identified as either statement or expression to be traversed over or captured.");

                if (kind == ElementKind.Expression)
                    return index;

                if (kind == ElementKind.Statement)
                    index -= 1;
            }
        }

        private void ImportPrecedingIntoBranches(IEnumerator<BranchStackFrame> stack) {
            if (!stack.MoveNext())
                throw new InvalidOperationException("There is no preceding expression in this context.");

            var frame = stack.Current;
            var targetIndex = frame.CurrentIndex - frame.Branching.ParameterCount;
            var index = this.FindPrecedingIndex(targetIndex, frame, stack);

            foreach (var branch in frame.Branching.GetBranches()) {
                branch.Insert(0, frame.Elements[index]);
            }
            frame.Elements.RemoveAt(index);
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