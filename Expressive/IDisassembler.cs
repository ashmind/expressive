using System.Collections.Generic;
using System.Reflection;
using Expressive.Abstraction;
using Expressive.Disassembly.Instructions;

namespace Expressive {
    public interface IDisassembler {
        IEnumerable<Instruction> Disassemble(IManagedMethod method);
    }
}