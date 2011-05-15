using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Expressive.Elements.Instructions;

namespace Expressive.Tests.Methods {
    public class TestDisassembler : Disassembler {
        public override IEnumerable<Instruction> Disassemble(MethodBase method) {
            var assembled = method as TestMethod;
            if (assembled != null)
                return assembled.GetInstructions();

            return base.Disassemble(method);
        }
    }
}
