using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressive.Disassembly.Instructions {
    public class BranchInstruction : Instruction {
        public int TargetOffset { get; private set; }

        public BranchInstruction(int offset, OpCode opCode, int targetOffset) : base(offset, opCode) {
            this.TargetOffset = targetOffset;
        }

        public override string ToString() {
            return base.ToString() + string.Format(" 0x{0:X2}", this.TargetOffset);
        }
    }
}
