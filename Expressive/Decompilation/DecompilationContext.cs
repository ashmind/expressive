using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Decompilation {
    public class DecompilationContext {
        public DecompilationContext(IDecompiler decompiler, MethodBase method, Func<int, Expression> getParameter) {
            this.Decompiler = decompiler;
            this.Method = method;
            this.GetParameter = getParameter;
        }

        public IDecompiler Decompiler { get; private set; }
        public MethodBase Method { get; private set; }
        public Func<int, Expression> GetParameter { get; private set; }
    }
}