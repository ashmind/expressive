using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ClrTest.Reflection;

using Expressive.Elements;

namespace Expressive {
    public class Disassembler : IDisassembler {
        public virtual IEnumerable<IElement> Disassemble(MethodBase method) {
            return new ILReader(method).Select(instruction => (IElement)new InstructionElement(instruction));
        }
    }
}
