using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Abstraction;
using Expressive.Disassembly;
using Expressive.Disassembly.Instructions;

namespace Expressive {
    public class Disassembler : IDisassembler {
        private readonly Func<byte[], IManagedContext, IInstructionReader> instructionReaderFactory;

        public Disassembler(Func<byte[], IManagedContext, IInstructionReader> instructionReaderFactory) {
            this.instructionReaderFactory = instructionReaderFactory;
        }

        public virtual IEnumerable<Instruction> Disassemble(IManagedMethod method) {
            return this.instructionReaderFactory(
                method.GetBodyByteArray(),
                method.Context
            ).ReadAll();
        }
    }
}
