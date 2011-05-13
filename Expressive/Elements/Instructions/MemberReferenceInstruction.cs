using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Expressive.Elements.Instructions {
    public class MemberReferenceInstruction : Instruction {
        public MemberInfo Member { get; private set; }

        public MemberReferenceInstruction(int offset, OpCode opCode, MemberInfo member) : base(offset, opCode) {
            this.Member = member;
        }
    }
}
