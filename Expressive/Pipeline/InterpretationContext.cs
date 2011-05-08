using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipeline {
    public class InterpretationContext {
        public InterpretationContext(MethodBase method, IList<IInterpretationStep> pipeline) {
            this.Method = method;
            this.Pipeline = pipeline;
            this.ExtractedParameters = new HashSet<ParameterExpression>();
        }

        public MethodBase Method { get; private set; }
        public IList<IInterpretationStep> Pipeline { get; private set; }
        public HashSet<ParameterExpression> ExtractedParameters { get; private set; }

        public void ApplyPipeline(IList<IElement> elements) {
            this.Pipeline.ForEach(p => p.Apply(elements, this));
        }
    }
}