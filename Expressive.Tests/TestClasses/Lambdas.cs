using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Lambdas {
        [ExpectedExpression("query => query.Where(c => (c.FirstName.Length > 5))")]
        public static IEnumerable<ClassWithNames> SimpleWhere(IEnumerable<ClassWithNames> query) {
            return query.Where(c => c.FirstName.Length > 5);
        }

        [ExpectedExpression("(query, length) => query.Where(Convert(CreateDelegate(System.Func`2[Expressive.Tests.TestClasses.ClassWithNames,System.Boolean], new {length = length}, Boolean <WhereWithClosureOverParameter>b__2(Expressive.Tests.TestClasses.ClassWithNames), True)))")]
        public static IEnumerable<ClassWithNames> WhereWithClosureOverParameter(IEnumerable<ClassWithNames> query, int length) {
            return query.Where(c => c.FirstName.Length > length);
        }
    }
}
