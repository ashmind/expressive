using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Elements.Expressions {
    public class ExpressionTreeVisitor : MsdnExamples.ExpressionVisitor {
        protected override Expression Visit(Expression exp) {
            var local = exp as LocalExpression;
            if (local != null)
                return this.VisitLocal(local);

            var adapter = exp as BooleanAdapterExpression;
            if (adapter != null)
                return this.VisitBooleanAdapter(adapter);

            return base.Visit(exp);
        }

        protected virtual Expression VisitLocal(LocalExpression local) {
            return local;
        }

        private Expression VisitBooleanAdapter(BooleanAdapterExpression adapter) {
            var expression = this.Visit(adapter.Expression);
            if (expression != adapter.Expression)
                return new BooleanAdapterExpression(expression);

            return adapter;
        }
    }
}
