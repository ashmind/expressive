using System;
using System.Collections.Generic;
using System.Linq;

using ClrTest.Reflection;

namespace Expressive.Abstraction {
    public interface IManagedMethod : IILProvider {
        IManagedMethodContext Context { get; }

        bool IsStatic { get; }
        Type DeclaringType { get; }

        Type ReturnType { get; }
        IEnumerable<IManagedMethodParameter> GetParameters();
        Type GetTypeOfLocal(int index);
    }
}
