using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class BrCuttingStep : IDecompilationStep {
        public void Apply(IList<IElement> elements, DecompilationContext context) {
            for (var i = elements.Count - 1; i >= 0; i--) {
                if (!BrProcessing.Matches(elements[i], code => code == OpCodes.Br || code == OpCodes.Br_S))
                    continue;

                var targetIndex = BrProcessing.FindTargetIndexOrThrow(elements[i], elements);
                BrProcessing.EnsureNotBackward(i, targetIndex);

                var skipStart = i + 1;
                var skipCount = targetIndex - skipStart;

                elements.RemoveAt(i);
                skipStart -= 1;
                if (skipCount == 0)
                    continue;

                var branch = elements.EnumerateRange(skipStart, skipCount).ToList();
                elements.RemoveRange(skipStart, skipCount);
                elements.Insert(skipStart, new CutBranchElement(branch));
            }
        }
    }
}
