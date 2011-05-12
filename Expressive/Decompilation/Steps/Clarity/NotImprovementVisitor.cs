using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.Clarity {
    public class NotImprovementVisitor : ElementVisitor {
        private static readonly Dictionary<ExpressionType, Func<BinaryExpression, BinaryExpression>> binaryInversions = new Dictionary<ExpressionType, Func<BinaryExpression, BinaryExpression>> {
            { ExpressionType.Equal, e => Expression.NotEqual(e.Left, e.Right) },
            { ExpressionType.LessThan, e => Expression.GreaterThanOrEqual(e.Left, e.Right) },
            { ExpressionType.GreaterThan, e => Expression.LessThanOrEqual(e.Left, e.Right) },
            { ExpressionType.LessThanOrEqual, e => Expression.GreaterThan(e.Left, e.Right) },
            { ExpressionType.GreaterThanOrEqual, e => Expression.LessThan(e.Left, e.Right) },
        };

        protected override Expression VisitUnary(UnaryExpression u) {
            u = (UnaryExpression)base.VisitUnary(u);
            if (u.NodeType != ExpressionType.Not)
                return u;

            if (u.Operand.NodeType == ExpressionType.Not)
                return ((UnaryExpression)u.Operand).Operand;

            var invert = binaryInversions.GetValueOrDefault(u.Operand.NodeType);
            if (invert == null)
                return u;

            return invert((BinaryExpression)u.Operand);
        }
    }
}
