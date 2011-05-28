using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Abstraction;
using Expressive.Disassembly;
using Expressive.Disassembly.Instructions;

namespace Expressive {
    public class Disassembler : IDisassembler {
        public virtual IEnumerable<Instruction> Disassemble(IManagedMethod method) {
            var bytes = method.GetByteArray();
            return new InstructionReader(bytes, method.Context).ReadAll();
        }
    }
}
