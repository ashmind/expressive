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

            return base.Visit(exp);
        }

        protected virtual Expression VisitLocal(LocalExpression local) {
            return local;
        }
    }
}
