using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expressive.Pipelines {
    public interface IDecompilationPipeline {
        IEnumerable<IInterpretationStep> GetSteps();
    }
}
