using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Nulls {
        [ExpectedExpression("(a, b) => (a ?? b)")]
        public static object CoalesceObjects(object a, object b) {
            return a ?? b;
        }

        [ExpectedExpression("(a, b) => IIF(a.HasValue, new Nullable`1(a.GetValueOrDefault()), b)")]
        public static int? CoalesceNullable(int? a, int? b) {
            return a ?? b;
        }

        [ExpectedExpression("(a, b) => (a ?? b)")]
        public static object IfNull(object a, object b) {
            return a != null ? a : b;
        }

        [ExpectedExpression("a => a.Value")]
        public static int CastFromNullable(int? a) {
            return (int)a;
        }

        [ExpectedExpression("a => new Nullable`1(a)")]
        public static int? CastToNullable(int a) {
            return a;
        }
    }
}
