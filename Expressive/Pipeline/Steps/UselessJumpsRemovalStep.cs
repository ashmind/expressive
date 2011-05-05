using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using AshMind.Extensions;
using ClrTest.Reflection;
using Expressive.Elements;

namespace Expressive.Pipeline.Steps {
    public class UselessJumpsRemovalStep : IInterpretationStep {
        public void Apply(InterpretationWorkspace workspace) {
            workspace.Elements.RemoveWhere((element, index) => IsUselessJump(element, index, workspace));
        }

        private bool IsUselessJump(IElement element, int index, InterpretationWorkspace workspace) {
            var jumpTarget = GetJumpTarget(element);
            if (jumpTarget == null)
                return false;

            return jumpTarget == GetOffset(workspace.Elements.ElementAtOrDefault(index + 1));
        }

        private int? GetJumpTarget(IElement element) {
            var instruction = element as InstructionElement;
            if (instruction == null)
                return null;

            if (instruction.OpCode == OpCodes.Br)
                return ((InlineBrTargetInstruction)instruction.Instruction).TargetOffset;

            if (instruction.OpCode == OpCodes.Br_S)
                return ((ShortInlineBrTargetInstruction)instruction.Instruction).TargetOffset;

            return null;
        }

        private int? GetOffset(IElement element) {
            var instruction = element as InstructionElement;
            return instruction != null ? instruction.Instruction.Offset : (int?) null;
        }
    }
}
