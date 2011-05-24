using System.Collections.Generic;
using System.Linq.Expressions;

using Expressive.Abstraction;

namespace Expressive {
    public interface IDecompiler {
        LambdaExpression Decompile(IManagedMethod method);
        Expression Decompile(IManagedMethod method, IList<Expression> arguments);
    }
}