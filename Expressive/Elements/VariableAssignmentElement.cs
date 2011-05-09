using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class VariableAssignmentElement : IElement {
        public int VariableIndex { get; private set; }
        public Expression Value  { get; private set; }

        public VariableAssignmentElement(int index, Expression value) {
            this.VariableIndex = index;
            this.Value = value;
        }

        public override string ToString() {
            return "local" + this.VariableIndex + " = " + this.Value;
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
