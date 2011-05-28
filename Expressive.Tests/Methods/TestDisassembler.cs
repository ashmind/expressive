using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Abstraction;
using Expressive.Disassembly;
using Expressive.Disassembly.Instructions;

namespace Expressive.Tests.Methods {
    public class TestDisassembler : Disassembler {
        public TestDisassembler(Func<byte[], IManagedMethodContext, IInstructionReader> instructionReaderFactory) : base(instructionReaderFactory) {
        }

        public override IEnumerable<Instruction> Disassemble(IManagedMethod method) {
            var assembled = method as TestMethod;
            if (assembled != null)
                return assembled.GetInstructions();

            return base.Disassemble(method);
        }
    }
}
