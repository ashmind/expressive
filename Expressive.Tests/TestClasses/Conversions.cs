using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Conversions {
        private const string Implicit = "x => x";

        #region Int32ToXXX

        [ExpectedExpression("x => (SByte)x")]
        public static sbyte Int32ToSByte(int x) {
            return (sbyte)x;
        }

        [ExpectedExpression("x => (Byte)x")]
        public static byte Int32ToByte(int x) {
            return (byte)x;
        }

        [ExpectedExpression("x => (Int16)x")]
        public static short Int32ToInt16(int x) {
            return (short)x;
        }

        [ExpectedExpression("x => (UInt16)x")]
        public static ushort Int32ToUInt16(int x) {
            return (ushort)x;
        }

        [ExpectedExpression(Implicit)]
        public static uint Int32ToUInt32(int x) {
            return (uint)x;
        }

        [ExpectedExpression("x => (Int64)x")]
        public static long Int32ToInt64(int x) {
            return x;
        }

        [ExpectedExpression("x => (Int64)x")]
        public static ulong Int32ToUInt64(int x) {
            return (ulong)x;
        }

        [ExpectedExpression("x => (Single)x")]
        public static float Int32ToSingle(int x) {
            return x;
        }

        [ExpectedExpression("x => (Double)x")]
        public static double Int32ToDouble(int x) {
            return x;
        }

        #endregion

        #region Int64ToXXX

        [ExpectedExpression("x => (SByte)x")]
        public static sbyte Int64ToSByte(long x) {
            return (sbyte)x;
        }

        [ExpectedExpression("x => (Byte)x")]
        public static byte Int64ToByte(long x) {
            return (byte)x;
        }

        [ExpectedExpression("x => (Int16)x")]
        public static short Int64ToInt16(long x) {
            return (short)x;
        }

        [ExpectedExpression("x => (UInt16)x")]
        public static ushort Int64ToUInt16(long x) {
            return (ushort)x;
        }

        [ExpectedExpression("x => (Int32)x")]
        public static int Int64ToInt32(long x) {
            return (int)x;
        }

        [ExpectedExpression("x => (UInt32)x")]
        public static uint Int64ToUInt32(long x) {
            return (uint)x;
        }

        [ExpectedExpression(Implicit)]
        public static ulong Int64ToUInt64(long x) {
            return (ulong)x;
        }

        [ExpectedExpression("x => (Single)x")]
        public static float Int64ToSingle(long x) {
            return x;
        }

        [ExpectedExpression("x => (Double)x")]
        public static double Int64ToDouble(long x) {
            return x;
        }

        #endregion

        #region DoubleToXXX

        [ExpectedExpression("x => (SByte)x")]
        public static sbyte DoubleToSByte(double x) {
            return (sbyte)x;
        }

        [ExpectedExpression("x => (Byte)x")]
        public static byte DoubleToByte(double x) {
            return (byte)x;
        }

        [ExpectedExpression("x => (Int16)x")]
        public static short DoubleToInt16(double x) {
            return (short)x;
        }

        [ExpectedExpression("x => (UInt16)x")]
        public static ushort DoubleToUInt16(double x) {
            return (ushort)x;
        }

        [ExpectedExpression("x => (Int32)x")]
        public static int DoubleToInt32(double x) {
            return (int)x;
        }

        [ExpectedExpression("x => (UInt32)x")]
        public static uint DoubleToUInt32(double x) {
            return (uint)x;
        }

        [ExpectedExpression("x => (Int64)x")]
        public static long DoubleToInt64(double x) {
            return (long)x;
        }

        [ExpectedExpression("x => (UInt64)x")]
        public static ulong DoubleToUInt64(double x) {
            return (ulong)x;
        }

        [ExpectedExpression("x => (Single)x")]
        public static float DoubleToSingle(double x) {
            return (float)x;
        }

        #endregion
    }
}
