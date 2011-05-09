using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Pipeline.Steps;
using Expressive.Pipeline.Steps.IndividualElements;
using Expressive.Pipeline.Steps.IndividualElements.Support;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            var decompiler = new Decompiler(
                new NopRemovalStep(),
                new BrCuttingStep(),
                new BrToJumpStep(),
                new CutBranchesRemovalStep(),
                new ElementInterpretationStep(
                    new LdstrToConstant(),
                    new CeqToCondition(),
                    new CallToExpression(),
                    new LdargToParameter()
                ),
                new JumpToExpressionStep(),
                new LdlocToVariableStep(),
                new StlocToAssignmentStep(),
                new VariableInliningStep(),
                new RetToReturnStep()
            );
            
            return decompiler.Decompile(method);
        }
    }
}
