using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class VariableAssignmentElement : IElement {
        public int VariableIndex { get; private set; }
        public Expression Value  { get; set; }

        public VariableAssignmentElement(int index, Expression value) {
            this.VariableIndex = index;
            this.Value = value;
        }

        public ElementKind Kind {
            get { return ElementKind.Statement; }
        }

        public override string ToString() {
            return "local" + this.VariableIndex + " = " + this.Value;
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
