using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Pipelines {
    public class DecompilationPipeline : IDecompilationPipeline {
        public IList<IInterpretationStep> Steps { get; private set; }

        protected DecompilationPipeline(params IInterpretationStep[] steps) {
            this.Steps = steps.ToList();
        }

        IEnumerable<IInterpretationStep> IDecompilationPipeline.GetSteps() {
            return this.Steps;
        }
    }
}
