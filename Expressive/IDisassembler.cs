using System.Collections.Generic;
using System.Reflection;
using Expressive.Abstraction;
using Expressive.Elements.Instructions;

namespace Expressive {
    public interface IDisassembler {
        IEnumerable<Instruction> Disassemble(IManagedMethod method);
    }
}