using System;
using System.Collections.Generic;
using System.Linq;

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

        public TElement CapturePreceding<TElement>() 
            where TElement : class, IElement
        {
            var preceding = this.GetPreceding<TElement>();
            var precedingIndex = this.CurrentIndex - 1;

            this.elements.RemoveAt(precedingIndex);
            this.CurrentIndex -= 1;
            return preceding;
        }

        public TElement GetPreceding<TElement>()
            where TElement : class, IElement
        {
            if (this.CurrentIndex == 0) {
                this.ImportPrecedingIntoBranches(this.branchStack.GetEnumerator());
                this.CurrentIndex += 1;
            }

            var preceding = this.elements[this.CurrentIndex - 1];
            return Cast<TElement>(preceding);
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

        private static TElement Cast<TElement>(IElement element)
            where TElement : class, IElement
        {
            var typed = element as TElement;
            if (typed == null)
                throw new InvalidOperationException("Element " + element + " must be an ExpressionElement to be used in this context.");
            return typed;
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