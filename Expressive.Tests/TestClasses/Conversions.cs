using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Conversions {
        private const string Explicit = "x => Convert(x)";
        private const string Implicit = "x => x";

        [ExpectedExpression(Explicit)]
        public static short Int32ToInt16(int x) {
            return (short)x;
        }

        [ExpectedExpression(Explicit)]
        public static ushort Int32ToUInt16(int x) {
            return (ushort)x;
        }

        [ExpectedExpression(Implicit)]
        public static uint Int32ToUInt32(int x) {
            return (uint)x;
        }

        [ExpectedExpression(Explicit)]
        public static long Int32ToInt64(int x) {
            return x;
        }

        [ExpectedExpression(Implicit)]
        public static uint Int32ToUInt64(int x) {
            return (uint)x;
        }

        [ExpectedExpression(Explicit)]
        public static float Int32ToSingle(int x) {
            return x;
        }

        [ExpectedExpression(Explicit)]
        public static double Int32ToDouble(int x) {
            return x;
        }
    }
}
