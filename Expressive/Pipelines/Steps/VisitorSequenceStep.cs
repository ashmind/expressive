using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipelines.Steps {
    public class VisitorSequenceStep : IInterpretationStep {
        private readonly ElementVisitor[] visitors;

        public VisitorSequenceStep(params ElementVisitor[] visitors) {
            this.visitors = visitors;
        }

        public void Apply(IList<IElement> elements, InterpretationContext context) {
            this.visitors.ForEach(v => v.VisitList(elements));
        }
    }
}
