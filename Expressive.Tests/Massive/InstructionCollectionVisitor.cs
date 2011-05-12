using System.Collections.Generic;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Tests.Massive {
    public class InstructionCollectingVisitor : ElementVisitor {
        public HashSet<OpCode> OpCodes { get; private set; }

        public InstructionCollectingVisitor() {
            this.OpCodes = new HashSet<OpCode>();
        }

        protected override IElement VisitInstruction(InstructionElement instruction) {
            this.OpCodes.Add(instruction.OpCode);
            return base.VisitInstruction(instruction);
        }
    }
}