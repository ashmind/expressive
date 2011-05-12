using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;
using Expressive.Elements.Presentation;

namespace Expressive.Pipelines.Steps.IndividualElements.Support {
    public class IndividualInterpretationContext {
        //private int lastRecursivePreprocessedIndex = 0;
        private readonly IList<IElement> elements;
        private readonly Action<IList<IElement>> interpret;

        public IndividualInterpretationContext(IList<IElement> elements, Action<IList<IElement>> interpret) {
            this.elements = elements;
            this.interpret = interpret;
        }

        public TElement CapturePreceding<TElement>()
            where TElement : class, IElement
        {
            return this.CapturePreceding<TElement>(-1);
        }

        public TElement CapturePreceding<TElement>(int negativeOffset) 
            where TElement : class, IElement
        {
            if (negativeOffset >= 0)
                throw new ArgumentOutOfRangeException("negativeOffset");

            //if (lastRecursivePreprocessedIndex < this.CurrentIndex)
            //    RecursiveProcessAllPreceding();

            var precedingIndex = this.CurrentIndex + negativeOffset;
            var preceding = this.elements[precedingIndex];
            var typed = preceding as TElement;
            if (typed == null)
                throw new InvalidOperationException("Element " + preceding + " must be an ExpressionElement to be used in this context.");

            this.elements.RemoveAt(precedingIndex);
            this.CurrentIndex -= 1;
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

        //private void RecursiveProcessAllPreceding() {
        //    var range = new PartialList<IElement>(
        //        this.elements, this.lastRecursivePreprocessedIndex,
        //        this.CurrentIndex - this.lastRecursivePreprocessedIndex
        //    );

        //    var countBeforeProcessing = range.Count;
        //    this.interpret(range);
        //    this.CurrentIndex += (range.Count - countBeforeProcessing);
        //    this.lastRecursivePreprocessedIndex = this.CurrentIndex;
        //}

        public int CurrentIndex { get; set; }
    }
}