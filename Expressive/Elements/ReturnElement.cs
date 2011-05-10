using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class ReturnElement : IElement {
        public Expression Result { get; private set; }

        public ReturnElement() : this(null) {
        }

        public ReturnElement(Expression result) {
            this.Result = result;
        }

        public override string ToString() {
            return "return" + (this.Result != null ? " " + this.Result : "");
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
