using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;

namespace Expressive.Pipeline.Steps {
    public class BooleanFixingVisitor : ElementVisitor {
        private readonly Stack<Expression> stack = new Stack<Expression>();
        
        private Expression Parent {
            get { return stack.ElementAtOrDefault(0); }
        }

        private bool ParentIsAdapter {
            get { return this.Parent is BooleanAdapterExpression; }
        }

        protected override Expression Visit(Expression exp) {
            this.stack.Push(exp);
            var result = base.Visit(exp);
            this.stack.Pop();
            return result;
        }

        protected override Expression VisitBooleanAdapter(BooleanAdapterExpression adapter) {
            var visited = base.Visit(adapter.Expression);
            return visited.Type == typeof(bool)
                 ? visited
                 : visited.NotEqual(Expression.Constant(0));
        }

        protected override Expression VisitConstant(ConstantExpression c) {
            if (this.ParentIsAdapter)
                return Expression.Constant(!Equals(c.Value, 0));

            return base.VisitConstant(c);
        }

        protected override Expression VisitConditional(ConditionalExpression c) {
            c = (ConditionalExpression)base.VisitConditional(c);
            if (!this.ParentIsAdapter)
                return c;

            var ifTrueAsConstant = c.IfTrue as ConstantExpression;
            var ifFalseAsConstant = c.IfFalse as ConstantExpression;
            if (ifTrueAsConstant == null || ifFalseAsConstant == null)
                return c;

            // seems it was something like 'ceq'

            var ifTrueIsTrue = !Equals(ifTrueAsConstant.Value, 0);
            var ifFalseIsTrue = !Equals(ifFalseAsConstant.Value, 0);

            if (ifTrueIsTrue == ifFalseIsTrue) // this should never happen
                return Expression.Constant(ifTrueIsTrue);

            return ifTrueIsTrue
                 ? c.Test
                 : Expression.Not(c.Test);
        }

        protected override Expression VisitBinary(BinaryExpression b) {
            var binary = (BinaryExpression)base.VisitBinary(b);
            if (binary.NodeType != ExpressionType.Equal && binary.NodeType != ExpressionType.NotEqual)
                return binary;

            if (binary.Left.Type != typeof(bool) || binary.Right.Type != typeof(bool))
                return binary;

            var leftAsConstant = binary.Left as ConstantExpression;
            var rightAsConstant = binary.Right as ConstantExpression;
            if (leftAsConstant == null && rightAsConstant == null)
                return binary;

            var @true = (bool)(leftAsConstant ?? rightAsConstant).Value;
            var equal = binary.NodeType == ExpressionType.Equal;

            var result = leftAsConstant == null ? binary.Left : binary.Right;
            return (@true == equal) // NOT XOR
                 ? result
                 : Expression.Not(result);
        }
    }
}
