﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressive.Disassembly.Instructions {
    public class MethodReferenceInstruction : Instruction {
        public MethodBase Method { get; private set; }

        public MethodReferenceInstruction(int offset, OpCode opCode, MethodBase method) : base(offset, opCode) {
            this.Method = method;
        }

        public override string ToString() {
            return base.ToString() + " " + this.Method;
        }
    }
}
