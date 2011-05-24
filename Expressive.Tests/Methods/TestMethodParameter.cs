using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Abstraction;

namespace Expressive.Tests.Methods {
    public class TestMethodParameter : IManagedMethodParameter {
        public TestMethodParameter(string name, Type parameterType) {
            this.Name = name;
            this.ParameterType = parameterType;
        }

        public string Name { get; private set; }
        public Type ParameterType { get; private set; }
    }
}
