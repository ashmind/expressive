using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Decompilation;
using Expressive.Decompilation.Pipelines;
using Expressive.Decompilation.Steps;
using Expressive.Decompilation.Steps.Clarity;
using Expressive.Decompilation.Steps.IndividualElements;
using Expressive.Decompilation.Steps.StatementInlining;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            var decompiler = new Decompiler(new DefaultPipeline());
            return decompiler.Decompile(method);
        }
    }
}
