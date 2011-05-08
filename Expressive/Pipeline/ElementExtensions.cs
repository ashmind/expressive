using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Pipeline {
    public static class ElementExtensions {
        public static OpCode? GetOpCodeIfInstruction(this IElement element) {
            return element.GetFromInstructionOrNull<OpCode?>(instruction => instruction.OpCode);
        }

        public static int? GetOffsetIfInstruction(this IElement element) {
            return element.GetFromInstructionOrNull<int?>(instruction => instruction.Offset);
        }

        public static T GetFromInstructionOrNull<T>(this IElement element, Func<InstructionElement, T> process) {
            var instruction = element as InstructionElement;
            return instruction != null ? process(instruction) : (T)(object)null;
        }
    }
}
