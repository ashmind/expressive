using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Decompilation.Pipelines {
    public interface IDecompilationPipeline {
        IEnumerable<IDecompilationStep> GetSteps();
    }
}
