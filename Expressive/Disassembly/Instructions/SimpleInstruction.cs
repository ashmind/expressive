using System.Reflection.Emit;

namespace Expressive.Disassembly.Instructions {
    public class SimpleInstruction : Instruction {
        public SimpleInstruction(int offset, OpCode opCode) : base(offset, opCode) {
        }
    }
}