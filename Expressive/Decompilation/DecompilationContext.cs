using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Expressive.Abstraction;

namespace Expressive.Decompilation {
    public class DecompilationContext {
        public DecompilationContext(IDecompiler decompiler, IManagedMethod method, Func<int, Expression> getParameter) {
            this.Decompiler = decompiler;
            this.Method = method;
            this.GetParameter = getParameter;
        }

        public IDecompiler Decompiler { get; private set; }
        public IManagedMethod Method { get; private set; }
        public Func<int, Expression> GetParameter { get; private set; }
    }
}