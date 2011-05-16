using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Decompilation {
    public class DecompilationContext {
        public DecompilationContext(MethodBase method, Func<int, Expression> getParameter) {
            this.Method = method;
            this.GetParameter = getParameter;
        }

        public MethodBase Method { get; private set; }
        public Func<int, Expression> GetParameter { get; private set; }
    }
}