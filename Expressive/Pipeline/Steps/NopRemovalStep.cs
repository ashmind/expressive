using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Pipeline {
    public class NopRemovalStep : IInterpretationStep {
        public void Apply(InterpretationWorkspace workspace) {
            workspace.Elements.RemoveWhere(e => e.GetOpCodeIfInstruction() == OpCodes.Nop);
        }
    }
}