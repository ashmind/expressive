using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class BrCuttingStep : IInterpretationStep {
        public void Apply(IList<IElement> elements, InterpretationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                if (!BrProcessing.In(elements[i], OpCodes.Br, OpCodes.Br_S))
                    continue;

                var targetIndex = BrProcessing.FindTargetIndexOrThrow(elements[i], elements);
                BrProcessing.EnsureNotBackward(i, targetIndex);

                var skipStart = i + 1;
                var skipCount = targetIndex - skipStart;

                elements.RemoveAt(i);
                i -= 1;
                skipStart -= 1;
                if (skipCount == 0)
                    continue;

                var branch = elements.EnumerateRange(skipStart, skipCount).ToArray();
                elements.RemoveRange(skipStart, skipCount);

                elements.Insert(skipStart, new CutBranchElement(branch));
            }
        }
    }
}
