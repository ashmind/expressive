using System.Collections.Generic;
using System.Reflection;

namespace Expressive.Tests.Methods {
    public class PseudoMethodBody : MethodBody {
        private readonly LocalVariableInfo[] localVariables;

        public PseudoMethodBody(LocalVariableInfo[] localVariables) {
            this.localVariables = localVariables;
        }

        public override IList<LocalVariableInfo> LocalVariables {
            get { return this.localVariables; }
        }
    }
}