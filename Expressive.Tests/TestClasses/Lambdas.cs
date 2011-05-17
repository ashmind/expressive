using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Lambdas {
        [ExpectedExpression("queryable => queryable.Where(c => (c.FirstName.Length > 5))")]
        public static IEnumerable<ClassWithNames> SimpleWhere(IEnumerable<ClassWithNames> queryable) {
            return queryable.Where(c => c.FirstName.Length > 5);
        }
    }
}
