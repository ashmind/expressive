using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Expressive.Elements;

namespace Expressive.Pipeline {
    public class InterpretationWorkspace {
        public InterpretationWorkspace(IList<IElement> elements, MethodBase method) {
            this.Elements = elements;
            this.Method = method;
            this.ExtractedParameters = new HashSet<ParameterExpression>();
        }

        public IList<IElement> Elements { get; private set; }
        public MethodBase Method { get; private set; }
        public HashSet<ParameterExpression> ExtractedParameters { get; private set; }
    }
}