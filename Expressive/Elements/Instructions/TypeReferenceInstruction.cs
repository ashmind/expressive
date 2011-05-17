using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public class TypeReferenceInstruction : Instruction {
        public Type Type { get; private set; }

        public TypeReferenceInstruction(int offset, OpCode opCode, Type type) : base(offset, opCode) {
            this.Type = type;
        }

        public override string ToString() {
            return base.ToString() + " " + this.Type;
        }
    }
}
