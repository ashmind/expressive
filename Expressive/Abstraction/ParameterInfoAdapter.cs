using System;
using System.Reflection;

namespace Expressive.Abstraction {
    public class ParameterInfoAdapter : IManagedMethodParameter {
        private readonly ParameterInfo actual;

        public ParameterInfoAdapter(ParameterInfo actual) {
            this.actual = actual;
        }

        public string Name {
            get { return this.actual.Name; }
        }

        public Type ParameterType {
            get { return this.actual.ParameterType; }
        }
    }
}