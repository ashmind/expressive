using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class CutBranchElement : IElement {
        public CutBranchElement(IList<IElement> elements) {
            this.Elements = elements;
        }

        public IList<IElement> Elements { get; private set; }

        public override string ToString() {
            return "cut branch";
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
