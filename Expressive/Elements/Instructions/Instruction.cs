using System;
using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public abstract class Instruction {
        public OpCode OpCode { get; private set; }
        public int Offset { get; private set; }

        protected Instruction(int offset, OpCode opCode) {
            this.Offset = offset;
            this.OpCode = opCode;
        }

        public override string ToString() {
            return this.OpCode.ToString();
        }
    }
}