using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public class SimpleInstruction : Instruction {
        public SimpleInstruction(int offset, OpCode opCode) : base(offset, opCode) {
        }
    }
}