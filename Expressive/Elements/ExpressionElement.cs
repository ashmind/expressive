using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class ExpressionElement : IElement {
        public Expression Expression { get; private set; }

        public ExpressionElement(Expression expression) {
            this.Expression = expression;
        }

        public override string ToString() {
            return this.Expression.ToString();
        }

        public string ToString(Indent indent) {
            return indent.Value + this;
        }
    }
}
