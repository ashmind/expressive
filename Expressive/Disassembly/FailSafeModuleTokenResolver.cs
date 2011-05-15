using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ClrTest.Reflection;

namespace Expressive.Disassembly {
    public class FailSafeModuleTokenResolver : ITokenResolver {
        private readonly Module module;
        private readonly Type[] genericMethodArguments;
        private readonly Type[] genericTypeArguments;

        public FailSafeModuleTokenResolver(MethodBase method) {
            this.module = method.Module;
            this.genericMethodArguments = (method.IsGenericMethod || method.IsGenericMethodDefinition) ? method.GetGenericArguments() : new Type[0];

            var type = method.DeclaringType;
            this.genericTypeArguments = (type != null && (type.IsGenericType || type.IsGenericTypeDefinition)) ? type.GetGenericArguments() : new Type[0];
        }

        public FieldInfo AsField(int token) {
            return this.module.ResolveField(token, this.genericTypeArguments, this.genericMethodArguments);
        }

        public MemberInfo AsMember(int token) {
            return this.module.ResolveMember(token, this.genericTypeArguments, this.genericMethodArguments);
        }

        public MethodBase AsMethod(int token) {
            return this.module.ResolveMethod(token, this.genericTypeArguments, this.genericMethodArguments);
        }

        public byte[] AsSignature(int token) {
            return this.module.ResolveSignature(token);
        }

        public string AsString(int token) {
            return this.module.ResolveString(token);
        }

        public Type AsType(int token) {
            return this.module.ResolveType(token, this.genericTypeArguments, this.genericMethodArguments);
        }
    }
}
