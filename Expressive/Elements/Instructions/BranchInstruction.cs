using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public class BranchInstruction : Instruction {
        public int TargetOffset { get; private set; }

        public BranchInstruction(int offset, OpCode opCode, int targetOffset) : base(offset, opCode) {
            this.TargetOffset = targetOffset;
        }
    }
}
