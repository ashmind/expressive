using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

using ClrTest.Reflection;

using Expressive.Disassembly.Instructions;

namespace Expressive.Disassembly {
    public class InstructionReader {
        private static readonly OpCode[] AllOpCodes = typeof(OpCodes).GetFields().Select(f => (OpCode)f.GetValue(null)).ToArray();
        private static readonly IDictionary<byte, IDictionary<ushort, OpCode>> OpCodesByFirstByte =
            AllOpCodes.GroupBy(o => o.Size > 1 ? (byte)(o.Value >> 8) : (byte)(o.Value & 0xFF))
                      .ToDictionary(g => g.Key, g => (IDictionary<ushort, OpCode>)g.ToDictionary(o => (ushort)o.Value));
        
        private static readonly IDictionary<Type, Delegate> ByteConverters = new Dictionary<Type, Delegate> {
            { typeof(sbyte),  ByteConverter((bytes, index) => (sbyte)bytes[index]) },    
            { typeof(byte),   ByteConverter((bytes, index) => bytes[index]) },
            { typeof(short),  ByteConverter(BitConverter.ToInt16)  },
            { typeof(ushort), ByteConverter(BitConverter.ToUInt16) },
            { typeof(int),    ByteConverter(BitConverter.ToInt32)  },
            { typeof(long),   ByteConverter(BitConverter.ToInt64)  },
            { typeof(float),  ByteConverter(BitConverter.ToSingle) },
            { typeof(double), ByteConverter(BitConverter.ToDouble) }
        };

        private int currentIndex;
        private readonly byte[] bytes;
        private readonly ITokenResolver resolver;

        public InstructionReader(byte[] bytes, ITokenResolver resolver) {
            this.bytes = bytes;
            this.resolver = resolver;
        }

        public IEnumerable<Instruction> ReadAll() {
            while (this.currentIndex < bytes.Length) {
                var first = bytes[this.currentIndex];
                var codes = OpCodesByFirstByte[first];
                OpCode code;
                if (codes.Count > 1) {
                    this.currentIndex += 1;
                    var second = bytes[this.currentIndex];
                    var value = (ushort)((first << 8) + second);
                    code = codes[value];
                }
                else {
                    code = codes.First().Value;
                }

                yield return ReadFrom(code);
            }
        }

        public Instruction ReadFrom(OpCode code) {
            var offset = this.currentIndex;
            this.currentIndex += 1;

            switch (code.OperandType) { // that is the first switch I write in 5 or 6 years
                case OperandType.InlineNone:
                    return new SimpleInstruction(offset, code);

                case OperandType.ShortInlineI:
                    return ReadValue<sbyte>(offset, code);

                case OperandType.InlineI:
                    return ReadValue<int>(offset, code);

                case OperandType.InlineI8:
                    return ReadValue<long>(offset, code);

                case OperandType.InlineR:
                    return ReadValue<double>(offset, code);

                case OperandType.ShortInlineR:
                    return ReadValue<float>(offset, code);

                case OperandType.InlineString:
                    return new ValueInstruction<string>(offset, code, this.resolver.AsString(Read<int>()));

                case OperandType.InlineVar:
                    return new VariableReferenceInstruction(offset, code, Read<ushort>());

                case OperandType.ShortInlineVar:
                    return new VariableReferenceInstruction(offset, code, Read<byte>());

                case OperandType.InlineBrTarget:
                    return ReadBranch<int>(offset, code);

                case OperandType.ShortInlineBrTarget:
                    return ReadBranch<sbyte>(offset, code);

                case OperandType.InlineMethod:
                    return new MethodReferenceInstruction(offset, code, this.resolver.AsMethod(Read<int>()));

                case OperandType.InlineField:
                    return new FieldReferenceInstruction(offset, code, this.resolver.AsField(Read<int>()));

                case OperandType.InlineType:
                    return new TypeReferenceInstruction(offset, code, this.resolver.AsType(Read<int>()));

                default:
                    throw new NotSupportedException(code.Name + " has unsupported operand type " + code.OperandType);
            }
        }

        private ValueInstruction<T> ReadValue<T>(int offset, OpCode code) {
            return new ValueInstruction<T>(offset, code, Read<T>());
        }

        private BranchInstruction ReadBranch<T>(int offset, OpCode code) 
            where T : IConvertible
        {
            var targetOffset = this.currentIndex + SizeOf<T>() + Read<T>().ToInt32(null);
            return new BranchInstruction(offset, code, targetOffset);
        }

        private T Read<T>() {
            var value = ((Func<byte[], int, T>)ByteConverters[typeof(T)]).Invoke(this.bytes, this.currentIndex);
            this.currentIndex += SizeOf<T>();
            return value;
        }

        private int SizeOf<T>() {
            return Marshal.SizeOf(typeof(T));
        }

        private static Func<byte[], int, T> ByteConverter<T>(Func<byte[], int, T> converter) {
            return converter;
        }
    }
}
