using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public class FieldReferenceInstruction : Instruction {
        public FieldInfo Field { get; private set; }

        public FieldReferenceInstruction(int offset, OpCode opCode, FieldInfo field) : base(offset, opCode) {
            this.Field = field;
        }
    }
}
