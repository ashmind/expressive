using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class VariableAssignmentElement : IElement {
        public int VariableIndex      { get; private set; }
        public IElement Value { get; private set; }

        public VariableAssignmentElement(int index, IElement value) {
            this.VariableIndex = index;
            this.Value = value;
        }

        public override string ToString() {
            return "var local" + this.VariableIndex + " = " + this.Value;
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
