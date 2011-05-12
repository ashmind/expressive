using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Decompilation {
    public class DecompilationContext {
        public DecompilationContext(MethodBase method) {
            this.Method = method;
            this.ExtractedParameters = new HashSet<ParameterExpression>();
        }

        public MethodBase Method { get; private set; }
        public HashSet<ParameterExpression> ExtractedParameters { get; private set; }
    }
}