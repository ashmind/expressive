using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Elements.Expressions {
    public class AddressOfExpression : Expression {
        public new const ExpressionType NodeType = (ExpressionType)2001;

        public MethodBase Method { get; private set; }

        public AddressOfExpression(MethodBase method) : base(NodeType, typeof(IntPtr)) {
            this.Method = method;
        }

        public override string ToString() {
            return "AddressOf " + this.Method;
        }
    }
}
