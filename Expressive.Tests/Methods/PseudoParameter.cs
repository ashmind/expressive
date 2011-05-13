using System;
using System.Reflection;

namespace Expressive.Tests.Methods {
    internal class PseudoParameter : ParameterInfo {
        public PseudoParameter(string name, Type type) {
            this.NameImpl = name;
            this.ClassImpl = type;
        }
    }
}