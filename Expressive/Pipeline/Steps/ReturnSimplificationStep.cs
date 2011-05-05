using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class ReturnSimplificationStep : IInterpretationStep {
        public void Apply(InterpretationWorkspace workspace) {
            if (workspace.Elements.Count > 1)
                return;

            var @return = workspace.Elements[0] as ReturnElement;
            if (@return == null)
                return;

            workspace.Elements[0] = @return.Result;
        }
    }
}
