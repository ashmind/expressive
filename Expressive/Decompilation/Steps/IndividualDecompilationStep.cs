using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Decompilation.Steps.IndividualElements;
using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class IndividualDecompilationStep : BranchingAwareStepBase {
        public IList<IElementInterpretation> Interpretations { get; private set; }

        public IndividualDecompilationStep(params IElementInterpretation[] interpretations) {
            this.Interpretations = interpretations.ToList();
        }

        public override void Apply(IList<IElement> elements, DecompilationContext context) {
            this.Interpretations.ForEach(x => x.Initialize(context));
            base.Apply(elements, context);
        }

        protected override void ApplyToSpecificElement(ref int index, IList<IElement> elements, Stack<BranchStackFrame> branchStack, DecompilationContext context) {
            var individualContext = new IndividualDecompilationContext(elements, branchStack);

            var element = elements[index];
            var indexFixed = index;
            var interpretation = this.Interpretations.FirstOrDefault(x => x.CanInterpret(elements[indexFixed]));
            if (interpretation == null)
                return;

            individualContext.CurrentIndex = index;
            var result = interpretation.Interpret(element, individualContext);
            index = individualContext.CurrentIndex; // some elements may have been captured, changing the collection, so index resync is needed
            if (result != null) {
                elements[index] = result;
            }
            else {
                elements.RemoveAt(index);
                index -= 1;
            }
        }
    }
}
