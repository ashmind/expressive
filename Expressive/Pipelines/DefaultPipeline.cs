using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Pipelines.Steps;
using Expressive.Pipelines.Steps.Clarity;
using Expressive.Pipelines.Steps.IndividualElements;
using Expressive.Pipelines.Steps.IndividualElements.Support;
using Expressive.Pipelines.Steps.StatementInlining;

namespace Expressive.Pipelines {
    public class DefaultPipeline : DecompilationPipeline {
        public DefaultPipeline() : base(
            new NopRemovalStep(),
            new BrCuttingStep(),
            new BrResolutionStep(),
            new CutBranchesRemovalStep(),
            new ElementInterpretationStep(
                new LdfldToField(),
                new LdstrToConstant(),
                new CeqToCondition(),
                new CallToExpression(),
                new LdlocToVariable(),
                new StlocToAssignment(),
                new LdargToParameter(),
                new LdcToConstant(),
                new BranchToCondition(),
                new RetToReturn()
            ),
            new VisitorSequenceStep(
                new IfToConditionVisitor(),
                new IfReturnInliningVisitor(),
                new ConditionImprovementVisitor(),
                new NotImprovementVisitor(),
                new BooleanEqualityImprovementVisitor()
            ),
            new VariableInliningStep()
        ) {
        }
    }
}
