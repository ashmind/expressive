using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Elements.Expressions {
    public class NewArrayWithSizeExpression : Expression {
        public new const ExpressionType NodeType = (ExpressionType)2002;

        public Expression Size { get; private set; }
        
        public NewArrayWithSizeExpression(Type type, Expression size) : base(NodeType, type) {
            this.Size = size;
        }

        public override string ToString() {
            return "new " + this.Type.FullName.TrimEnd(']') + this.Size + "]";
        }
    }
}
