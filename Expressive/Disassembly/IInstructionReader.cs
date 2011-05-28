using System.Collections.Generic;
using Expressive.Disassembly.Instructions;

namespace Expressive.Disassembly {
    public interface IInstructionReader {
        IEnumerable<Instruction> ReadAll();
    }
}