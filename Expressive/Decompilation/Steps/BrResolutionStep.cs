using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class BrResolutionStep : IDecompilationStep {
        private static readonly HashSet<OpCode> conditionalOpCodes = new HashSet<OpCode> {
            OpCodes.Brfalse, OpCodes.Brfalse_S,
            OpCodes.Brtrue,  OpCodes.Brtrue_S,
            OpCodes.Beq,     OpCodes.Beq_S,
            OpCodes.Bge,     OpCodes.Bge_S,     OpCodes.Bge_Un,  OpCodes.Bge_Un_S,
            OpCodes.Bgt,     OpCodes.Bgt_S,     OpCodes.Bgt_Un,  OpCodes.Bgt_Un_S,
            OpCodes.Ble,     OpCodes.Ble_S,     OpCodes.Ble_Un,  OpCodes.Ble_Un_S,
            OpCodes.Blt,     OpCodes.Blt_S,     OpCodes.Blt_Un,  OpCodes.Blt_Un_S,
                                                OpCodes.Bne_Un,  OpCodes.Bne_Un_S
        };

        public void Apply(IList<IElement> elements, DecompilationContext context) {
            for (var i = 0; i < elements.Count; i++) {
                var element = elements[i];
                if (!BrProcessing.Matches(element, conditionalOpCodes.Contains))
                    continue;

                var targetIndex = BrProcessing.FindTargetIndexOrNull(element, elements);
                if (targetIndex != null) {
                    ProcessJumpToExistingCode(targetIndex.Value, elements, context, ref i);
                    continue;
                }

                var targetCutBranch = elements.Select((e, index) => new { branch = e as CutBranchElement, index })
                                              .Where(x => x.branch != null)
                                              .Select(x => new {
                                                  indexOfBranch = x.index,
                                                  indexWithinBranch = BrProcessing.FindTargetIndexOrNull(element, x.branch.Elements),
                                                  elements = x.branch.Elements
                                              })
                                              .Where(x => x.indexWithinBranch != null)
                                              .FirstOrDefault();

                if (targetCutBranch == null)
                    BrProcessing.ThrowTargetNotFound(element);

                ProcessJumpToCutBranch(
                    targetCutBranch.indexOfBranch,
                    targetCutBranch.indexWithinBranch.Value,
                    targetCutBranch.elements,
                    elements,
                    context,
                    ref i
                );
            }
        }

        private void ProcessJumpToExistingCode(int targetIndex, IList<IElement> elements, DecompilationContext context, ref int currentIndex) {
            BrProcessing.EnsureNotBackward(currentIndex, targetIndex);
            var followingRange = elements.EnumerateRange(currentIndex + 1, targetIndex - (currentIndex + 1)).ToList();
            this.Apply(followingRange, context);

            ReplaceWithJumpUpTo(
                targetIndex - 1,
                elements,
                followingRange,
                new IElement[0],
                ref currentIndex
            );
        }

        private void ProcessJumpToCutBranch(int branchIndex, int targetIndex, IList<IElement> branch, IList<IElement> elements, DecompilationContext context, ref int currentIndex) {
            var followingRange = elements.EnumerateRange(currentIndex + 1, branchIndex - (currentIndex + 1)).ToList();
            var targetRange = branch.EnumerateRange(targetIndex, branch.Count - targetIndex).ToList();
            this.Apply(followingRange, context);
            this.Apply(targetRange, context);

            ReplaceWithJumpUpTo(
                branchIndex,
                elements,
                followingRange,
                targetRange,
                ref currentIndex
            );
        }

        private static void ReplaceWithJumpUpTo(
            int replaceUpTo,
            IList<IElement> elements,
            IList<IElement> following,
            IList<IElement> target,
            ref int currentIndex
        ) {
            var jump = new ConditionalBranchElement(
                ((InstructionElement)elements[currentIndex]).OpCode,
                target, following
            );

            elements.RemoveRange(currentIndex, (replaceUpTo + 1) - currentIndex);
            elements.Insert(currentIndex, jump);
        }
    }
}
