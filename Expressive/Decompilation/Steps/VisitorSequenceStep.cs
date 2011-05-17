using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class VisitorSequenceStep : IDecompilationStep {
        public IList<Func<DecompilationContext, ElementVisitor>> Visitors { get; private set; }

        public VisitorSequenceStep(params Func<DecompilationContext, ElementVisitor>[] visitors) {
            this.Visitors = visitors.ToList();
        }

        public void Apply(IList<IElement> elements, DecompilationContext context) {
            this.Visitors.ForEach(visitor => visitor(context).VisitList(elements));
        }
    }
}
