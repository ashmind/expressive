using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Constants {
        [ExpectedExpression("() => 10")]
        public static sbyte SByte() {
            return 10;
        }

        [ExpectedExpression("() => 10")]
        public static byte Byte() {
            return 10;
        }

        [ExpectedExpression("() => 1000")]
        public static short Int16() {
            return 1000;
        }

        [ExpectedExpression("() => 1000")]
        public static ushort UInt16() {
            return 1000;
        }

        [ExpectedExpression("() => 100000")]
        public static int Int32() {
            return 100000;
        }

        [ExpectedExpression("() => 100000")]
        public static uint UInt32() {
            return 100000;
        }

        [ExpectedExpression("() => 10000000000")]
        public static long Int64() {
            return 10000000000;
        }

        [ExpectedExpression("() => 10000000000")]
        public static ulong UInt64() {
            return 10000000000;
        }

        [ExpectedExpression("() => 1")]
        public static float Single() {
            return 1.0F;
        }

        [ExpectedExpression("() => 1")]
        public static double Double() {
            return 1.0D;
        }

        [ExpectedExpression("() => \"test\"")]
        public static string String() {
            return "test";
        }

        // [ExpectedExpression("() => 'x'")]
        // not working as expected yet
        public static char Char() {
            return 'x';
        }
    }
}
