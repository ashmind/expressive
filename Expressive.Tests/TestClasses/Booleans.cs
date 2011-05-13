using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Expressive.Tests.Methods;

namespace Expressive.Tests.TestClasses {
    public static class Booleans {
        [ExpectedExpression("(a, b) => (a AndAlso b)")]
        public static bool And(bool a, bool b) {
            return a && b;
        }

        [ExpectedExpression("(a, b) => (a OrElse b)")]
        public static bool Or(bool a, bool b) {
            return a || b;
        }

        [ExpectedExpression("(a, b) => ((a == b) OrElse ((a != null) AndAlso ((b != null) AndAlso a.Equals(b))))")]
        public static bool ObjectEquals(object a, object b) {
            return a == b
                || (
                    (a != null) && (b != null) && a.Equals(b)
                );
        }

        //[ExpectedExpression("(a, b) => ((a == b) OrElse ((a != null) AndAlso ((b != null) AndAlso a.Equals(b))))")]
        //Not working yet, work in progress
        public static readonly AssembledMethod OtherEquals = new AssembledMethod("AssembledEquals")
            .Parameter<object>("a")
            .Parameter<object>("b")
            .Assemble(a => a
                .Ldarg_0
                .Ldarg_1
                .Bne_Un_S(0x6)
                .Ldc_I4_1
                .Ret
                .Ldarg_0
                .Brfalse_S(0xC)
                .Ldarg_1
                .Brtrue_S(0xE)
                .Ldc_I4_0
                .Ret
                .Ldarg_0
                .Ldarg_1
                .Callvirt<object>(o => o.Equals(null))
                .Ret
            );
    }
}
