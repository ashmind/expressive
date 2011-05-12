using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Decompilation.Pipelines {
    public class DecompilationPipeline : IDecompilationPipeline {
        public IList<IDecompilationStep> Steps { get; private set; }

        protected DecompilationPipeline(params IDecompilationStep[] steps) {
            this.Steps = steps.ToList();
        }

        IEnumerable<IDecompilationStep> IDecompilationPipeline.GetSteps() {
            return this.Steps;
        }
    }
}
