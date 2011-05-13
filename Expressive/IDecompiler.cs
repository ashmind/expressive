using System.Linq.Expressions;
using System.Reflection;

namespace Expressive {
    public interface IDecompiler {
        LambdaExpression Decompile(MethodBase method);
    }
}