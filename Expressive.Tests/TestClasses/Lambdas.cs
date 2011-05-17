using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Lambdas {
        [ExpectedExpression("queryable => queryable.Where(Convert(CreateDelegate(System.Func`2[Expressive.Tests.TestClasses.ClassWithNames,System.Boolean], null, Boolean <SimpleWhere>b__0(Expressive.Tests.TestClasses.ClassWithNames), True)))")]
        public static IEnumerable<ClassWithNames> SimpleWhere(IEnumerable<ClassWithNames> queryable) {
            return queryable.Where(c => c.FirstName.Length > 5);
        }
    }
}
