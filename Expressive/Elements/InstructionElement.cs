using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using ClrTest.Reflection;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class InstructionElement : IElement { 
        public ILInstruction Instruction { get; private set; }

        public OpCode OpCode {
            get { return this.Instruction.OpCode; }
        }

        public int Offset {
            get { return this.Instruction.Offset; }
        }

        public InstructionElement(ILInstruction instruction) {
            this.Instruction = instruction;
        }
        
        public override string ToString() {
            return this.OpCode.ToString();
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
