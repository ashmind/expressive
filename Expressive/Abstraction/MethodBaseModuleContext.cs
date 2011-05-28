using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressive.Abstraction {
    public class MethodBaseModuleContext : IManagedContext {
        private readonly Module module;
        private readonly Type[] methodGenericArguments;
        private readonly Type[] typeGenericArguments;

        public MethodBaseModuleContext(MethodBase method) {
            this.module = method.Module;
            this.methodGenericArguments = (method.IsGenericMethod || method.IsGenericMethodDefinition) ? method.GetGenericArguments() : new Type[0];

            var type = method.DeclaringType;
            this.typeGenericArguments = (type != null && (type.IsGenericType || type.IsGenericTypeDefinition)) ? type.GetGenericArguments() : new Type[0];
        }

        public FieldInfo ResolveField(int token) {
            return this.module.ResolveField(token, this.typeGenericArguments, this.methodGenericArguments);
        }

        public MemberInfo ResolveMember(int token) {
            return this.module.ResolveMember(token, this.typeGenericArguments, this.methodGenericArguments);
        }

        public MethodBase ResolveMethod(int token) {
            return this.module.ResolveMethod(token, this.typeGenericArguments, this.methodGenericArguments);
        }

        public string ResolveString(int token) {
            return this.module.ResolveString(token);
        }

        public Type ResolveType(int token) {
            return this.module.ResolveType(token, this.typeGenericArguments, this.methodGenericArguments);
        }
    }
}
