using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Decompilation {
    public class DecompilationContext {
        #region ParameterExpressionCollection Class

        public class ParameterExpressionCollection : KeyedCollection<string, ParameterExpression> {
            protected override string GetKeyForItem(ParameterExpression item) {
                return item.Name;
            }
        }

        #endregion

        public DecompilationContext(MethodBase method) {
            this.Method = method;
            this.ExtractedParameters = new ParameterExpressionCollection();
        }

        public MethodBase Method { get; private set; }
        public ParameterExpressionCollection ExtractedParameters { get; private set; }
    }
}