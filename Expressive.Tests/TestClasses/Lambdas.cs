using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Lambdas {
        [ExpectedExpression("query => query.Where(c => (c.FirstName.Length > 5))")]
        public static IEnumerable<ClassWithNames> SimpleWhere(IEnumerable<ClassWithNames> query) {
            return query.Where(c => c.FirstName.Length > 5);
        }

        //[ExpectedExpression("")]
        // Work in progress: depends on initializers
        public static IEnumerable<ClassWithNames> WhereWithClosureOverParameter(IEnumerable<ClassWithNames> query, int length) {
            return query.Where(c => c.FirstName.Length > length);
        }
    }
}
