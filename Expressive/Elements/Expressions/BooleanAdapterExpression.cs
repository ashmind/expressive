using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Elements.Expressions {
    public class BooleanAdapterExpression : Expression {
        public new const ExpressionType NodeType = (ExpressionType)8001;
        public Expression Expression { get; private set; }

        public BooleanAdapterExpression(Expression expression) : base(NodeType, typeof(bool)) {
            this.Expression = expression;
        }

        public override string ToString() {
            return "booleanize(" + this.Expression + ")";
        }
    }
}
