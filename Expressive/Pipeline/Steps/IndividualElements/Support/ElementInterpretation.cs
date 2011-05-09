using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps.IndividualElements.Support {
    public abstract class ElementInterpretation<TInputElement, TResultElement> : IElementInterpretation 
        where TInputElement : class, IElement
        where TResultElement : IElement
    {
        public virtual void Initialize(InterpretationContext context) {
        }

        bool IElementInterpretation.CanInterpret(IElement element) {
            var input = element as TInputElement;
            if (input == null)
                return false;

            return this.CanInterpret(input);
        }

        IElement IElementInterpretation.Interpret(IElement element, IndividualInterpretationContext context) {
            return this.Interpret((TInputElement)element, context);
        }

        public virtual bool CanInterpret(TInputElement element) {
            return true;
        }

        public abstract TResultElement Interpret(TInputElement element, IndividualInterpretationContext context);
    }
}
