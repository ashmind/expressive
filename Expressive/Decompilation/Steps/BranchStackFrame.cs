using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class BranchStackFrame {
        public IList<IElement> Elements { get; private set; }
        public BranchingElement Branching { get; private set; }
        public int CurrentIndex { get; set; }

        public BranchStackFrame(IList<IElement> elements, BranchingElement branching, int currentIndex) {
            this.Elements = elements;
            this.Branching = branching;
            this.CurrentIndex = currentIndex;
        }
    }
}
