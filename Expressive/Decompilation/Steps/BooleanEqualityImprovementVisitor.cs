using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps {
    public class BooleanEqualityImprovementVisitor : ElementVisitor {
        protected override Expression VisitBinary(BinaryExpression b) {
            b = (BinaryExpression) base.VisitBinary(b);
            if (b.Left.Type != typeof(bool) || b.Right.Type != typeof(bool))
                return b;

            if (b.NodeType != ExpressionType.Equal && b.NodeType != ExpressionType.NotEqual)
                return b;

            var leftAsConstant = b.Left as ConstantExpression;
            var rightAsConstant = b.Right as ConstantExpression;
            if (leftAsConstant == null && rightAsConstant == null)
                return b;

            return rightAsConstant != null
                 ? Calculate(b.Left, b.NodeType, (bool)rightAsConstant.Value)
                 : Calculate(b.Right, b.NodeType, (bool)leftAsConstant.Value);
        }

        private Expression Calculate(Expression expression, ExpressionType binaryType, bool constant) {
            if (binaryType == ExpressionType.NotEqual)
                constant = !constant;

            return constant ? expression : Expression.Not(expression);
        }
    }
}
