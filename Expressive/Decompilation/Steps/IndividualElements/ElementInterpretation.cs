using System;
using System.Collections.Generic;
using System.Linq;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps.IndividualElements {
    public abstract class ElementInterpretation<TInputElement, TResultElement> : IElementInterpretation 
        where TInputElement : class, IElement
        where TResultElement : IElement
    {
        public virtual void Initialize(DecompilationContext context) {
        }

        bool IElementInterpretation.CanInterpret(IElement element) {
            var input = element as TInputElement;
            if (input == null)
                return false;

            return this.CanInterpret(input);
        }

        IElement IElementInterpretation.Interpret(IElement element, IndividualDecompilationContext context) {
            return this.Interpret((TInputElement)element, context);
        }

        public abstract bool CanInterpret(TInputElement element);
        public abstract TResultElement Interpret(TInputElement element, IndividualDecompilationContext context);
    }
}
