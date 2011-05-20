using System;
using System.Collections.Generic;
using System.Linq;

using AshMind.Extensions;

using Expressive.Decompilation.Steps;
using Expressive.Decompilation.Steps.Clarity;
using Expressive.Decompilation.Steps.IndividualElements;
using Expressive.Decompilation.Steps.StatementInlining;
using Expressive.Decompilation.Steps.StatementInlining.AssignmentInlining;
using Expressive.Decompilation.Steps.StatementInlining.InitializerCollectors;

namespace Expressive.Decompilation.Pipelines {
    public class DefaultPipeline : DecompilationPipeline {
        public DefaultPipeline() : base(
            new NopRemovalStep(),
            new BranchResolutionStep(),
            new UnconditionalBranchesRemovalStep(),
            new IndividualDecompilationStep(
                new LdfldToField(),
                new LdstrToConstant(),
                new LdftnToAddressOf(),
                new LdnullToConstant(),
                new ConvToConvert(),
                new CxxToCondition(),
                new MathToExpression(),
                new CallToElement(),
                new NewobjToNew(),
                new NewarrToNewArray(),
                new LdlocToVariable(),
                new StlocToAssignment(),
                new StfldToAssignment(),
                new StelemToAssignment(),
                new LdargToParameter(),
                new LdcToConstant(),
                new BranchToCondition(),
                new DupToCopy(),
                new PopToRemove(),
                new RetToReturn()
            ),
            new VisitorSequenceStep(
                c => new AddressOfToCreateDelegateVisitor(),
                c => new IfThenCollapsingVisitor(), // must be before following two
                c => new IfAssignmentInliningVisitor(),
                c => new IfReturnInliningVisitor(),
                c => new ConditionImprovementVisitor(),
                c => new BooleanEqualityImprovementVisitor(),
                c => new NotImprovementVisitor(),
                c => new InitializerDetectingVisitor(
                    new AssignmentInliner(),
                    new ObjectInitializerCollector(),
                    new ArrayInitializerCollector(),
                    new CollectionInitializerCollector()
                ),
                c => new NewNullableToCastVisitor()
            ),
            new VariableInliningStep(new AssignmentInliner()),
            new VisitorSequenceStep(
                c => new CoalescingVisitor(),
                c => new LambdaInliningVisitor(c)
            )
        ) {
        }

        public virtual DefaultPipeline Without<TPipelinePart>() {
            this.Steps.RemoveWhere(s => s is TPipelinePart);
            this.Steps.ForEach(this.RemoveFromStep<TPipelinePart>);

            return this;
        }

        protected virtual void RemoveFromStep<TPipelinePart>(IDecompilationStep step) {
            var individual = step as IndividualDecompilationStep;
            if (individual != null) {
                individual.Interpretations.RemoveWhere(i => i is TPipelinePart);
                return;
            }

            var sequence = step as VisitorSequenceStep;
            if (sequence != null) {
                sequence.Visitors.RemoveWhere(v => v is TPipelinePart);
                return;
            }
        }
    }
}
