using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Initializers {
        public class SimpleClass {
            public object A { get; set; }
            public object B { get; set; }
        }

        //[ExpectedExpression("?")]
        // Work in progress
        public static object[] ObjectArray(object a, object b) {
            return new[] { a, b };
        }

        //[ExpectedExpression("?")]
        public static SimpleClass ClassWithProperties(object a, object b) {
            return new SimpleClass { A = a, B = b };
        }
    }
}
