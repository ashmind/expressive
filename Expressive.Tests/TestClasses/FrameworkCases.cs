using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class FrameworkCases {
        // [ExpectedExpression("(a, b) => ((a == b) || (a != null) && (b != null) && a.Equals(b))")]
        // work in progress, fails for now
        public static bool ObjectEquals(object a, object b) {
            return a == b
                || (
                    (a != null) && (b != null) && a.Equals(b)
                );
        }
    }
}
