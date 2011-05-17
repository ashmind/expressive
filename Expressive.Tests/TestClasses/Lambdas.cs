using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Expressive.Tests.TestClasses {
    public static class Lambdas {
        //[ExpectedExpression("?")]
        //Work in progress
        public static IEnumerable<ClassWithNames> SimpleWhere(IEnumerable<ClassWithNames> queryable) {
            return queryable.Where(c => c.FirstName.Length > 5);
        }

        public static void Test() {
            Expression<Func<object, Func<object, bool>>> f = o => o.Equals;
        }
    }
}
