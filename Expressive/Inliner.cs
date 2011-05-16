using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Inlining;

namespace Expressive {
    public class Inliner : IInliner {
        private readonly IDecompiler decompiler;

        public Inliner(IDecompiler decompiler) {
            this.decompiler = decompiler;
        }

        public TExpression Inline<TExpression>(TExpression expression, Func<MemberInfo, bool> shouldInline) 
            where TExpression : Expression
        {
            return new InliningVisitor(this.decompiler, shouldInline).Inline(expression);
        }
    }
}
