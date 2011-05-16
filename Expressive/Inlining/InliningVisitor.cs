using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using MsdnExamples;

namespace Expressive.Inlining {
    public class InliningVisitor : ExpressionVisitor {
        private readonly IDecompiler decompiler;
        private readonly Func<MemberInfo, bool> shouldInline;

        public InliningVisitor(IDecompiler decompiler, Func<MemberInfo, bool> shouldInline) {
            this.decompiler = decompiler;
            this.shouldInline = shouldInline;
        }

        public TExpression Inline<TExpression>(TExpression expression) 
            where TExpression : Expression
        {
            return (TExpression)this.Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m) {
            m = (MethodCallExpression)base.VisitMethodCall(m);
            if (!shouldInline(m.Method))
                return m;

            var argumentList = (IList<Expression>)m.Arguments;
            if (m.Object != null) {
                argumentList = argumentList.ToList();
                argumentList.Insert(0, m.Object);
            }

            return this.decompiler.Decompile(m.Method, argumentList);
        }

        protected override Expression VisitMemberAccess(MemberExpression m) {
            m = (MemberExpression)base.VisitMemberAccess(m);
            var property = m.Member as PropertyInfo;
            if (property == null)
                return m;

            var getter = property.GetGetMethod();
            if (!shouldInline(property) && (getter == null || !shouldInline(getter)))
                return m;

            return this.decompiler.Decompile(getter, new[] { m.Expression });
        }
    }
}