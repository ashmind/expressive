using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Pipeline;
using Expressive.Pipeline.Steps;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            var decompiler = new Decompiler(
                new UselessJumpsRemovalStep(),
                new NopRemovalStep(),
                new LdargToParameterStep(),
                new LdstrToConstantStep(),
                new LdlocToVariableStep(),
                new CallToExpressionStep(),
                new StlocToAssignmentStep(),
                new VariableInliningStep(),
                new RetToReturnStep(),
                new ReturnSimplificationStep()
            );
            
            return decompiler.Decompile(method);
        }
    }
}
