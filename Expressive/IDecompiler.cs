using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive {
    public interface IDecompiler {
        LambdaExpression Decompile(MethodBase method);
        Expression Decompile(MethodBase method, IList<Expression> arguments);
    }
}