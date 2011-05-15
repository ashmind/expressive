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

        [ExpectedExpression("() => Or(True, False)")]
        public static bool CallWithConstants() {
            return Or(true, false);
        }

        private const string ObjectEqualsExpression1 = "(a, b) => ((a == b) OrElse ((a != null) AndAlso ((b != null) AndAlso a.Equals(b))))";
        private const string ObjectEqualsExpression2 = "(a, b) => ((a == b) OrElse (((a != null) AndAlso (b != null)) AndAlso a.Equals(b)))";

        [ExpectedExpression(ObjectEqualsExpression1, ObjectEqualsExpression2)]
        public static bool ObjectEquals(object a, object b) {
            return a == b
                || (
                    (a != null) && (b != null) && a.Equals(b)
                );
        }

        [ExpectedExpression(ObjectEqualsExpression1, ObjectEqualsExpression2)]
        public static readonly MethodBase ObjectEqualsRelease = new TestMethodBuilder()
            .Name("ObjectEquals{Release}")
            .Parameter<object>("a")
            .Parameter<object>("b")
            .Assemble(a => a
                .Ldarg_0
                .Ldarg_1
                .Beq_S(0x14)
                .Ldarg_0
                .Brfalse_S(0x12)
                .Ldarg_1
                .Brfalse_S(0x12)
                .Ldarg_0
                .Ldarg_1
                .Callvirt<object>(o => o.Equals(null))
                .Ret
                .Ldc_I4_0
                .Ret
                .Ldc_I4_1
                .Ret
            )
            .Returns<bool>()
            .ToMethod();

        [ExpectedExpression(ObjectEqualsExpression1, ObjectEqualsExpression2)]
        public static readonly MethodBase OtherEquals = new TestMethodBuilder()
            .Name("AssembledEquals")
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
            )
            .Returns<bool>()
            .ToMethod();
    }
}
