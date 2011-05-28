using System.Reflection.Emit;

namespace Expressive.Disassembly.Instructions {
    public class VariableReferenceInstruction : Instruction {
        public ushort Ordinal { get; private set; }

        public VariableReferenceInstruction(int offset, OpCode opCode, ushort ordinal) : base(offset, opCode) {
            this.Ordinal = ordinal;
        }

        public override string ToString() {
            return base.ToString() + " " + this.Ordinal;
        }
    }
}