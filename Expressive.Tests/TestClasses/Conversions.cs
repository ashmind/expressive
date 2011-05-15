using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Conversions {
        private const string Convert = "x => Convert(x)";

        [ExpectedExpression(Convert)]
        public static long Int32ToInt64(int x) {
            return x;
        }

        [ExpectedExpression(Convert)]
        public static short Int32ToInt16(int x) {
            return (short)x;
        }

        [ExpectedExpression(Convert)]
        public static float Int32ToSingle(int x) {
            return x;
        }

        [ExpectedExpression(Convert)]
        public static double Int32ToDouble(int x) {
            return x;
        }
    }
}
