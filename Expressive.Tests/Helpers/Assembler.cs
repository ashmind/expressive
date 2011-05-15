using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;

using AshMind.Extensions;

using Expressive.Elements.Instructions;

namespace Expressive.Tests.Helpers {
    public class Assembler {
        private readonly IDictionary<OperandType, int> OperandSizes = new Dictionary<OperandType, int> {
            { OperandType.InlineBrTarget,      4 },
            { OperandType.InlineField,         4 },
            { OperandType.InlineI,             4 },
            { OperandType.InlineI8,            8 },
            { OperandType.InlineMethod,        4 },
            { OperandType.InlineR,             8 },
            { OperandType.InlineSig,           4 },
            { OperandType.InlineString,        4 },
            { OperandType.InlineSwitch,        4 },
            { OperandType.InlineTok,           4 },
            { OperandType.InlineType,          4 },
            { OperandType.InlineVar,           2 },
            { OperandType.ShortInlineBrTarget, 1 },
            { OperandType.ShortInlineI,        1 },
            { OperandType.ShortInlineR,        4 },
            { OperandType.ShortInlineVar,      1 }
        };

        private int offset = 0;
        private readonly IList<Instruction> instructions = new List<Instruction>();

        public static Assembler Start {
            get { return new Assembler(); }
        }

        private Assembler Append(Instruction instruction) {
            this.instructions.Add(instruction);
            this.offset += instruction.OpCode.Size + OperandSizes.GetValueOrDefault(instruction.OpCode.OperandType);
            return this;
        }

        private Assembler Simple(OpCode opCode) {
            return this.Append(new SimpleInstruction(this.offset, opCode));
        }

        private Assembler Branch(OpCode opCode, int targetOffset) {
            return this.Append(new BranchInstruction(this.offset, opCode, targetOffset));
        }

        public IList<Instruction> End {
            get { return this.instructions; }
        }

        public Assembler Nop {
            get { return Simple(OpCodes.Nop); }
        }

        public Assembler Ldarg_0 {
            get { return Simple(OpCodes.Ldarg_0); }
        }

        public Assembler Ldarg_1 {
            get { return Simple(OpCodes.Ldarg_1); }
        }

        public Assembler Ldc_I4_0 {
            get { return Simple(OpCodes.Ldc_I4_0); }
        }

        public Assembler Ldc_I4_1 {
            get { return Simple(OpCodes.Ldc_I4_1); }
        }

        public Assembler Stloc_0 {
            get { return Simple(OpCodes.Stloc_0); }
        }

        public Assembler Ldloc_0 {
            get { return Simple(OpCodes.Ldloc_0); }
        }

        public Assembler Ret {
            get { return Simple(OpCodes.Ret); }
        }

        public Assembler Br_S(int targetOffset) {
            return Branch(OpCodes.Br_S, targetOffset);
        }

        public Assembler Beq_S(int targetOffset) {
            return Branch(OpCodes.Beq_S, targetOffset);
        }

        public Assembler Bne_Un_S(int targetOffset) {
            return Branch(OpCodes.Bne_Un_S, targetOffset);
        }

        public Assembler Brtrue_S(int targetOffset) {
            return Branch(OpCodes.Brtrue_S, targetOffset);
        }

        public Assembler Brfalse_S(int targetOffset) {
            return Branch(OpCodes.Brfalse_S, targetOffset);
        }

        public Assembler Callvirt<T>(Expression<Action<T>> call) {
            return Append(new MethodReferenceInstruction(
                this.offset, OpCodes.Callvirt,
                Method.Get(call)
            ));
        }
    }
}
