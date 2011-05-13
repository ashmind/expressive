using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;

using AshMind.Extensions;

namespace Expressive.Elements.Instructions {
    public class SwitchInstruction : Instruction {
        public ReadOnlyCollection<int> TargetOffsets { get; set; }

        public SwitchInstruction(int offset, OpCode opCode, int[] targetOffsets) : base(offset, opCode) {
            TargetOffsets = targetOffsets.AsReadOnly();
        }
    }
}
