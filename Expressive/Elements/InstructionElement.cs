using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using ClrTest.Reflection;
using Expressive.Disassembly.Instructions;
using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class InstructionElement : IElement, IPreservingOffset { 
        public Instruction Instruction { get; private set; }

        public InstructionElement(Instruction instruction) {
            this.Instruction = instruction;
        }

        public OpCode OpCode {
            get { return this.Instruction.OpCode; }
        }

        public int Offset {
            get { return this.Instruction.Offset; }
        }

        public ElementKind Kind {
            get { return ElementKind.Undefined; }
        }
        
        public override string ToString() {
            return this.Instruction.ToString();
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
