using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;
using Expressive.Elements.Presentation;

namespace Expressive.Decompilation.Steps {
    public class IndividualDecompilationContext {
        private readonly IList<IElement> elements;

        public IndividualDecompilationContext(IList<IElement> elements) {
            this.elements = elements;
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

        public int CurrentIndex { get; set; }
    }
}