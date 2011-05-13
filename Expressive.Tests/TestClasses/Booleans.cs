using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
