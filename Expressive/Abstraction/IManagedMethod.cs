using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Abstraction {
    public interface IManagedMethod {
        IManagedContext Context { get; }

        bool IsStatic { get; }
        Type DeclaringType { get; }

        Type ReturnType { get; }
        IEnumerable<IManagedMethodParameter> GetParameters();
        Type GetTypeOfLocal(int index);

        byte[] GetBodyByteArray();
    }
}
