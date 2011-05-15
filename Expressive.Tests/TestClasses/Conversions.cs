using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Conversions {
        [ExpectedExpression("x => Convert(x)")]
        public static long Int32ToInt64(int x) {
            return x;
        }

        [ExpectedExpression("x => Convert(x)")]
        public static short Int32ToInt16(int x) {
            return (short)x;
        }
    }
}
