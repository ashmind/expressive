using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Initializers {
        // [ExpectedExpression("?")]
        // Work in progress
        public static object[] ObjectArray(object a, object b) {
            return new[] { a, b };
        }
    }
}
