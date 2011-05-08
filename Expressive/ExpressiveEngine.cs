using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Pipeline.Steps;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            var decompiler = new Decompiler(
                new NopRemovalStep(),
                new BrCuttingStep(),
                new BrToJumpStep(),
                new CutBranchesRemovalStep(),
                new JumpToExpressionStep(),
                new LdargToParameterStep(),
                new LdstrToConstantStep(),
                new LdlocToVariableStep(),
                new CallToExpressionStep(),
                new StlocToAssignmentStep(),
                new VariableInliningStep(),
                new RetToReturnStep()
            );
            
            return decompiler.Decompile(method);
        }
    }
}
