using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClrTest.Reflection;
using Expressive.Disassembly;
using Expressive.Elements.Instructions;

namespace Expressive {
    public class Disassembler : IDisassembler {
        public virtual IEnumerable<Instruction> Disassemble(MethodBase method) {
            return new ILReader(
                new MethodBaseILProvider(method),
                new FailSafeModuleTokenResolver(method)
            ).Select(Resolve);
        }

        private Instruction Resolve(ILInstruction instruction) {
            return ResolveAs<InlineNoneInstruction>(instruction, x => new SimpleInstruction(x.Offset, x.OpCode))

                ?? ResolveAs<InlineBrTargetInstruction>(instruction, br => new BranchInstruction(br.Offset, br.OpCode, br.TargetOffset))
                ?? ResolveAs<ShortInlineBrTargetInstruction>(instruction, br => new BranchInstruction(br.Offset, br.OpCode, br.TargetOffset))

                ?? ResolveAs<InlineFieldInstruction>(instruction, f => new FieldReferenceInstruction(f.Offset, f.OpCode, f.Field))
                ?? ResolveAs<InlineMethodInstruction>(instruction, m => new MethodReferenceInstruction(m.Offset, m.OpCode, m.Method))
                ?? ResolveAs<InlineTokInstruction>(instruction, m => new MemberReferenceInstruction(m.Offset, m.OpCode, m.Member))
                ?? ResolveAs<InlineTypeInstruction>(instruction, t => new TypeReferenceInstruction(t.Offset, t.OpCode, t.Type))
                ?? ResolveAs<InlineVarInstruction>(instruction, v => new VariableReferenceInstruction(v.Offset, v.OpCode, v.Ordinal))
                ?? ResolveAs<ShortInlineVarInstruction>(instruction, v => new VariableReferenceInstruction(v.Offset, v.OpCode, v.Ordinal))

                ?? ResolveAs<ShortInlineIInstruction>(instruction, x => new ValueInstruction<byte>(x.Offset, x.OpCode, x.Byte))
                ?? ResolveAs<InlineIInstruction>(instruction, x => new ValueInstruction<int>(x.Offset, x.OpCode, x.Int32))
                ?? ResolveAs<InlineI8Instruction>(instruction, x => new ValueInstruction<long>(x.Offset, x.OpCode, x.Int64))
                ?? ResolveAs<ShortInlineRInstruction>(instruction, x => new ValueInstruction<float>(x.Offset, x.OpCode, x.Single))
                ?? ResolveAs<InlineRInstruction>(instruction, x => new ValueInstruction<double>(x.Offset, x.OpCode, x.Double))
                ?? ResolveAs<InlineStringInstruction>(instruction, x => new ValueInstruction<string>(x.Offset, x.OpCode, x.String))

                ?? ResolveAs<InlineSwitchInstruction>(instruction, s => new SwitchInstruction(s.Offset, s.OpCode, s.TargetOffsets))

                ?? ResolveAs<InlineSigInstruction>(instruction, x => new UnsupportedInstruction(x.Offset, x.OpCode));
        }

        private Instruction ResolveAs<TILnstruction>(ILInstruction instruction, Func<TILnstruction, Instruction> resolve) 
            where TILnstruction : ILInstruction
        {
            var typed = instruction as TILnstruction;
            if (typed == null)
                return null;

            return resolve(typed);
        }
    }
}
