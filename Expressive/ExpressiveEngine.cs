using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Decompilation.Pipelines;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            return new Decompiler(new DefaultPipeline()).Decompile(method);
        }
    }
}
