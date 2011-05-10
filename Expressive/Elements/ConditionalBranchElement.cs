using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class ConditionalBranchElement : IBranchingElement {
        public OpCode OpCode  { get; set; }
        public IList<IElement> Target  { get; private set; }
        public IList<IElement> Fallback { get; private set; }

        public ConditionalBranchElement(OpCode opCode, IList<IElement> target, IList<IElement> fallback) {
            this.OpCode = opCode;
            this.Target = target;
            this.Fallback = fallback;
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var builder = new StringBuilder();
            var innerIndent = indent.Increment();
            builder.Append(indent.Value)
                   .AppendFormat("branch ({0}):", this.OpCode)
                   .AppendLine()
                   .Append(innerIndent.Value)
                   .Append("target:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.Target, innerIndent.Increment()));
            
            builder.AppendLine()
                   .Append(innerIndent.Value)
                   .Append("fallback:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.Fallback, innerIndent.Increment()));
            return builder.ToString();
        }

        public IEnumerable<IList<IElement>> GetBranches() {
            yield return this.Target;
            yield return this.Fallback;
        }
    }
}
