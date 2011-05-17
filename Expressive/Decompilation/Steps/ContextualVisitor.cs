using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class ContextualVisitor : ElementVisitor {
        public DecompilationContext Context { get; private set; }

        public ContextualVisitor(DecompilationContext context) {
            this.Context = context;
        }
    }
}
