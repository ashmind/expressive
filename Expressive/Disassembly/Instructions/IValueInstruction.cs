using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Disassembly.Instructions {
    public interface IValueInstruction {
        object Value { get; }
    }
}
