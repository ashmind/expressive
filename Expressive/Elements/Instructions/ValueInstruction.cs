using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public class ValueInstruction<T> : Instruction, IValueInstruction {
        public T Value { get; private set; }

        public ValueInstruction(int offset, OpCode opCode, T value) : base(offset, opCode) {
            this.Value = value;
        }

        #region IValueInstruction Members

        object IValueInstruction.Value {
            get { return this.Value; }
        }

        #endregion
    }
}
