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

        public static void AdaptIfRequired(ref Expression left, ref Expression right) {
            left = AdaptIfRequired(left, right.Type);
            right = AdaptIfRequired(right, left.Type);
        }

        public static Expression AdaptIfRequired(Expression expression, Type requiredType) {
            if (requiredType != typeof(bool))
                return expression;

            return expression.Type == typeof(bool)
                 ? expression
                 : new BooleanAdapterExpression(expression);
        }
    }
}
