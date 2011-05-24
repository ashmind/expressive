using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Lambdas {
        [ExpectedExpression("query => query.Where(c => (c.FirstName.Length > 5))")]
        public static IEnumerable<ClassWithNames> SimpleLambda(IEnumerable<ClassWithNames> query) {
            return query.Where(c => c.FirstName.Length > 5);
        }

        [ExpectedExpression("(query, length) => query.Where(c => (c.FirstName.Length > length))")]
        public static IEnumerable<ClassWithNames> LambdaWithClosureOverParameter(IEnumerable<ClassWithNames> query, int length) {
            return query.Where(c => c.FirstName.Length > length);
        }

        [ExpectedExpression("() => Lambdas.Return(() => Lambdas.Return(() => Lambdas.Return(() => null)))")]
        public static object LambdaWithinLambdaWithnLambda() {
            return Return(() => Return(() => Return(() => null)));
        }

        private static object Return(Func<object> func) {
            return func();
        }
    }
}
