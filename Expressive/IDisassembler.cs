using System.Collections.Generic;
using System.Reflection;
using Expressive.Elements;

namespace Expressive {
    public interface IDisassembler {
        IEnumerable<IElement> Disassemble(MethodBase method);
    }
}