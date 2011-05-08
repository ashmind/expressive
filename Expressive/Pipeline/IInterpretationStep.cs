using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Pipeline {
    public interface IInterpretationStep {
        void Apply(IList<IElement> elements, InterpretationContext context);
    }
}
