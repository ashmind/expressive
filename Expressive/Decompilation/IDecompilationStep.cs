using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Decompilation {
    public interface IDecompilationStep {
        void Apply(IList<IElement> elements, DecompilationContext context);
    }
}
