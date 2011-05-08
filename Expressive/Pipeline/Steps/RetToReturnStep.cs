using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class RetToReturnStep : BranchingAwareStepBase {
        protected override void ApplyToSpecificBranch(IList<IElement> elements, InterpretationContext context) {
            var method = context.Method as MethodInfo;
            var hasReturnValue = method != null && method.ReturnType != typeof(void);

            for (var i = 0; i < elements.Count; i++) {
                if (elements[i].GetOpCodeIfInstruction() != OpCodes.Ret)
                    continue;

                var result = (IElement)null;
                if (hasReturnValue) {
                    result = elements[i - 1];
                    elements.RemoveAt(i - 1);
                    i -= 1;
                }

                elements[i] = new ReturnElement(result);
            }
        }
    }
}
