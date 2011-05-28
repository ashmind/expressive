using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressive.Abstraction {
    public interface IManagedContext {
        MethodBase ResolveMethod(int token);
        FieldInfo ResolveField(int token);
        Type ResolveType(int token);
        string ResolveString(int token);
        MemberInfo ResolveMember(int token);
    }
}
