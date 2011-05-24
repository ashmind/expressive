using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Abstraction {
    public interface IManagedMethodParameter {
        string Name { get; }
        Type ParameterType { get; }
    }
}
