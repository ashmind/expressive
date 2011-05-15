using System;
using System.Reflection;

namespace Expressive.Tests.Methods {
    public class PseudoLocalVariable : LocalVariableInfo {
        private readonly Type type;

        public PseudoLocalVariable(Type type) {
            this.type = type;
        }

        public override Type LocalType {
            get { return this.type; }
        }
    }
}