using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Pipeline {
    public interface IInterpretationStep {
        void Apply(InterpretationWorkspace workspace);
    }
}
