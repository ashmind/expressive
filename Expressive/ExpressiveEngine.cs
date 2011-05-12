using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Pipelines;
using Expressive.Pipelines.Steps;
using Expressive.Pipelines.Steps.Clarity;
using Expressive.Pipelines.Steps.IndividualElements;
using Expressive.Pipelines.Steps.IndividualElements.Support;
using Expressive.Pipelines.Steps.StatementInlining;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            var decompiler = new Decompiler(new DefaultPipeline());
            return decompiler.Decompile(method);
        }
    }
}
