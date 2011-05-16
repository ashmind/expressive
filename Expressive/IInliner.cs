using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive {
    public interface IInliner {
        TExpression Inline<TExpression>(TExpression expression, Func<MemberInfo, bool> shouldInline)
            where TExpression : Expression;
    }
}