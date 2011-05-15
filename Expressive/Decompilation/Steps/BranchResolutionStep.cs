using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class BranchResolutionStep : IDecompilationStep {
        private static readonly HashSet<OpCode> opCodes = new HashSet<OpCode> {
            OpCodes.Br,      OpCodes.Br_S,
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
            var results = new List<IElement>();
            this.ProcessRange(elements, 0, results, new Dictionary<IElement, IElement>());
            elements.Clear();
            elements.AddRange(results);
        }

        private void ProcessRange(IList<IElement> elements, int startIndex, IList<IElement> results, IDictionary<IElement, IElement> original) {
            for (var i = startIndex; i < elements.Count; i++) {
                var element = elements[i];
                if (!BranchProcessing.Matches(element, opCodes.Contains)) {
                    results.Add(element);
                    original[element] = element;
                    continue;
                }

                var targetIndexOrNull = BranchProcessing.FindTargetIndexOrNull(element, elements);
                if (targetIndexOrNull == null)
                    BranchProcessing.ThrowTargetNotFound(element);

                var targetIndex = targetIndexOrNull.Value;
                BranchProcessing.EnsureNotBackward(i, targetIndex);

                var targetRange = new List<IElement>();
                this.ProcessRange(elements, targetIndex, targetRange, original);

                var followingRange = new List<IElement>();
                this.ProcessRange(elements, i + 1, followingRange, original);

                var convergingRange = new List<IElement>();
                while (targetRange.Count > 0 && followingRange.Count > 0) {
                    var lastTarget = targetRange.Last();
                    var lastFollowing = followingRange.Last();
                    if (original[lastTarget] != original[lastFollowing])
                        break;

                    convergingRange.Add(lastFollowing);
                    targetRange.RemoveAt(targetRange.Count - 1);
                    followingRange.RemoveAt(followingRange.Count - 1);
                }
                convergingRange.Reverse();

                if (targetRange.Count > 0 || followingRange.Count > 0) {
                    var branching = new BranchingElement(((InstructionElement)element).OpCode, targetRange, followingRange);
                    original[branching] = element;
                    results.Add(branching);
                }

                results.AddRange(convergingRange);
                break;
            }
        }
    }
}
