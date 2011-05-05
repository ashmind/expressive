using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class RetToReturnStep : IInterpretationStep {
        public void Apply(InterpretationWorkspace workspace) {
            var method = workspace.Method as MethodInfo;
            var hasReturnValue = method != null && method.ReturnType != typeof(void);

            for (var i = 0; i < workspace.Elements.Count; i++) {
                if (workspace.Elements[i].GetOpCodeIfInstruction() != OpCodes.Ret)
                    continue;

                var result = (IElement)null;
                if (hasReturnValue) {
                    result = workspace.Elements[i - 1];
                    workspace.Elements.RemoveAt(i - 1);
                    i -= 1;
                }

                workspace.Elements[i] = new ReturnElement(result);
            }
        }
    }
}
