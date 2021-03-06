﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expressive.Abstraction {
    public class MethodBaseAdapter : IManagedMethod {
        private readonly MethodBase actual;
        private readonly MethodBody bodyOfActual;

        public MethodBaseAdapter(MethodBase actual) {
            this.actual = actual;
            this.bodyOfActual = this.actual.GetMethodBody();
            if (bodyOfActual == null)
                throw new NotSupportedException("Method " + actual + " must have a body to adapted using this class.");

            this.Context = new MethodBaseModuleContext(actual);
        }

        public IManagedContext Context { get; private set; }

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

        public byte[] GetBodyByteArray() {
            return this.bodyOfActual.GetILAsByteArray();
        }

        public override string ToString() {
            return this.actual.ToString();
        }
    }
}
