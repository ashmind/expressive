using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public static class BrProcessing {
        public static bool Matches(IElement element, Func<OpCode, bool> predicate) {
            var instruction = element as InstructionElement;
            if (instruction == null)
                return false;

            return predicate(instruction.OpCode);
        }

        public static int GetTargetOffset(IElement br) {
            var instruction = ((InstructionElement)br).Instruction;

            var @short = instruction as ShortInlineBrTargetInstruction;
            if (@short != null)
                return @short.TargetOffset;

            return ((InlineBrTargetInstruction)instruction).TargetOffset;
        }

        public static int FindTargetIndexOrThrow(IElement br, IList<IElement> elements) {
            var targetIndex = FindTargetIndexOrNull(br, elements);
            if (targetIndex == null)
                BrProcessing.ThrowTargetNotFound(br);

            return (int)targetIndex;
        }

        public static int? FindTargetIndexOrNull(IElement br, IList<IElement> elements) {
            var targetOffset = GetTargetOffset(br);
            for (var i = 0; i < elements.Count; i++) {
                var withOffset = elements[i] as IPreservingOffset;
                if (withOffset != null && withOffset.Offset == targetOffset)
                    return i;
            }
            return null;
        }

        public static void ThrowTargetNotFound(IElement br) {
            throw new InvalidOperationException(
                "After previous steps, cannot find jump target that was at 0x" + GetTargetOffset(br).ToString("X") + "."
            );
        }

        public static void EnsureNotBackward(int index, int targetIndex) {
            if (targetIndex < index)
                throw new NotSupportedException("Backward jumps are currently not supported.");
        }
    }
}
