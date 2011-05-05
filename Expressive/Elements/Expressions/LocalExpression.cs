using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Elements.Expressions {
    public class LocalExpression : Expression {
        public static readonly new ExpressionType NodeType = (ExpressionType)2000;
        
        public int Index { get; set; }
        
        public LocalExpression(int index, Type type) : base(NodeType, type) {
            this.Index = index;
        }

        public override string ToString() {
            return "local" + this.Index;
        }
    }
}
