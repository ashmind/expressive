using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Abstraction;
using Expressive.Disassembly;
using Expressive.Disassembly.Instructions;

namespace Expressive {
    public class Disassembler : IDisassembler {
        private readonly Func<byte[], IManagedMethodContext, IInstructionReader> instructionReaderFactory;

        public Disassembler(Func<byte[], IManagedMethodContext, IInstructionReader> instructionReaderFactory) {
            this.instructionReaderFactory = instructionReaderFactory;
        }

        public virtual IEnumerable<Instruction> Disassemble(IManagedMethod method) {
            var bytes = method.GetByteArray();
            return this.instructionReaderFactory(bytes, method.Context).ReadAll();
        }
    }
}
