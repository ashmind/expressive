﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressive.Disassembly.Instructions {
    public class UnsupportedInstruction : Instruction {
        public UnsupportedInstruction(int offset, OpCode opCode) : base(offset, opCode) {
        }
    }
}
