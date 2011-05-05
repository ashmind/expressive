using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Pipeline {
    public static class ElementExtensions {
        public static OpCode? GetOpCodeIfInstruction(this IElement element) {
            var instruction = element as InstructionElement;
            return instruction != null ? instruction.OpCode : (OpCode?)null;
        }
    }
}
