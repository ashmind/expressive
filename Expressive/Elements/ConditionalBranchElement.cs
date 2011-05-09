using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class ConditionalBranchElement : IBranchingElement {
        public IList<IElement> IfTrue { get; private set; }
        public IList<IElement> IfFalse { get; private set; }

        public ConditionalBranchElement(IList<IElement> ifTrue, IList<IElement> ifFalse) {
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var builder = new StringBuilder();
            builder.Append(indent.Value)
                   .Append("if true:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.IfTrue, indent.Increment()));
            
            builder.AppendLine()
                   .Append(indent.Value)
                   .Append("if false:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.IfFalse, indent.Increment()));
            return builder.ToString();
        }

        public IEnumerable<IList<IElement>> GetBranches() {
            yield return this.IfTrue;
            yield return this.IfFalse;
        }
    }
}
