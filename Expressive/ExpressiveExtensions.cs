using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Abstraction;
using Expressive.Disassembly.Instructions;

namespace Expressive {
    public static class ExpressiveExtensions {
        public static LambdaExpression Decompile(this IDecompiler decompiler, MethodBase method) {
            return decompiler.Decompile(new MethodBaseAdapter(method));
        }

        public static Expression Decompile(this IDecompiler decompiler, MethodBase method, IList<Expression> arguments) {
            return decompiler.Decompile(new MethodBaseAdapter(method), arguments);
        }

        public static IEnumerable<Instruction> Disassemble(this IDisassembler disassembler, MethodBase method) {
            return disassembler.Disassemble(new MethodBaseAdapter(method));
        }
    }
}
