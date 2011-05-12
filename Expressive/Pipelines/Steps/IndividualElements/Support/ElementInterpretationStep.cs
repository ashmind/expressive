using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipelines.Steps.IndividualElements.Support {
    public class ElementInterpretationStep : BranchingAwareStepBase {
        private readonly IElementInterpretation[] interpretations;

        public ElementInterpretationStep(params IElementInterpretation[] interpretations) {
            this.interpretations = interpretations;
        }

        public override void Apply(IList<IElement> elements, InterpretationContext context) {
            this.interpretations.ForEach(x => x.Initialize(context));
            base.Apply(elements, context);
        }

        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            var individualContext = new IndividualInterpretationContext(elements, range => this.Apply(range, context));
            for (var i = 0; i < elements.Count; i++) {
                var element = elements[i];
                var interpretation = this.interpretations.FirstOrDefault(x => x.CanInterpret(elements[i]));
                if (interpretation == null)
                    continue;

                individualContext.CurrentIndex = i;
                var result = interpretation.Interpret(element, individualContext);
                i = individualContext.CurrentIndex; // some elements may have been captured, changing the collection, so index resync is needed
                elements[i] = result;
            }
        }
    }
}
