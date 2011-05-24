using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClrTest.Reflection;

namespace Expressive.Abstraction {
    public class MethodBaseAdapter : IManagedMethod {
        private readonly MethodBase actual;
        private readonly MethodBody bodyOfActual;

        public MethodBaseAdapter(MethodBase actual) {
            this.actual = actual;
            this.bodyOfActual = this.actual.GetMethodBody();
            this.Context = new MethodBaseModuleContext(actual);
        }

        public IManagedMethodContext Context { get; private set; }

        public bool IsStatic {
            get { return this.actual.IsStatic; }
        }

        public Type DeclaringType {
            get { return this.actual.DeclaringType; }
        }

        public Type ReturnType {
            get {
                var info = this.actual as MethodInfo;
                return info != null ? info.ReturnType : null;
            }
        }

        public IEnumerable<IManagedMethodParameter> GetParameters() {
            return this.actual.GetParameters().Select(p => (IManagedMethodParameter)new ParameterInfoAdapter(p));
        }

        public Type GetTypeOfLocal(int index) {
            return this.bodyOfActual.LocalVariables[index].LocalType;
        }

        #region IILProvider Members

        byte[] IILProvider.GetByteArray() {
            return this.bodyOfActual.GetILAsByteArray();
        }

        #endregion
    }
}
