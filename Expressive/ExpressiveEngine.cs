using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Pipeline.Steps;
using Expressive.Pipeline.Steps.IndividualElements;
using Expressive.Pipeline.Steps.IndividualElements.Support;
using Expressive.Pipeline.Steps.Optimizations;

namespace Expressive {
    public static class ExpressiveEngine {
        public static LambdaExpression ToExpression(MethodBase method) {
            var decompiler = new Decompiler(
                new NopRemovalStep(),
                new BrCuttingStep(),
                new BrResolutionStep(),
                new CutBranchesRemovalStep(),
                new ElementInterpretationStep(
                    new LdstrToConstant(),
                    new CeqToCondition(),
                    new CallToExpression(),
                    new LdlocToVariable(),
                    new StlocToAssignment(),
                    new LdargToParameter(),
                    new LdcToConstant(),
                    new BranchToCondition()
                ),
                new IfOptimizationStep(),
                new VariableInliningStep(),
                new RetToReturnStep()
            );
            
            return decompiler.Decompile(method);
        }
    }
}
