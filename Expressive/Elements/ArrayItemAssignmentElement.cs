using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class ArrayItemAssignmentElement : IElement {
        public Expression Array { get; set; }
        public Expression Index { get; set; }
        public Expression Value { get; set; }

        public ArrayItemAssignmentElement(Expression array, Expression index, Expression value) {
            this.Array = array;
            this.Index = index;
            this.Value = value;
        }

        public ElementKind Kind {
            get { return ElementKind.Statement; }
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            return string.Format(
                "{0}{1}[{2}] = {3}",
                    indent.Value, this.Array, this.Index, this.Value
            );
        }
    }
}
