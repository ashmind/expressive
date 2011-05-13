using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Numerics {
        [ExpectedExpression("(a, b) => (a + b)")]
        public static int Add(int a, int b) {
            return a + b;
        }

        [ExpectedExpression("(a, b) => (a - b)")]
        public static int Subtract(int a, int b) {
            return a - b;
        }

        [ExpectedExpression("(a, b) => (a & b)")]
        public static int And(int a, int b) {
            return a & b;
        }

        [ExpectedExpression("(a, b) => (a | b)")]
        public static int Or(int a, int b) {
            return a | b;
        }
    }
}
