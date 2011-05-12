using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Pipelines {
    public class InterpretationContext {
        public InterpretationContext(MethodBase method) {
            this.Method = method;
            this.ExtractedParameters = new HashSet<ParameterExpression>();
        }

        public MethodBase Method { get; private set; }
        public HashSet<ParameterExpression> ExtractedParameters { get; private set; }
    }
}