using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public static class Initializers {
        public class SimpleClass {
            public object FieldA;
            public object FieldB;

            public object PropertyA { get; set; }
            public object PropertyB { get; set; }

            public SimpleClass Recursive { get; set; }
        }

        [ExpectedExpression("(a, b) => new [] { a, b }")]
        public static object[] ObjectArray(object a, object b) {
            return new[] { a, b };
        }

        [ExpectedExpression("(a, b) => new [] { a, b }")]
        public static int[] Int32Array(int a, int b) {
            return new[] { a, b };
        }

        [ExpectedExpression("(a, b) => new List<Object> { a, b }")]
        public static IList<object> ObjectList(object a, object b) {
            return new List<object> { a, b };
        }

        [ExpectedExpression("(key, value) => new Dictionary<Object, Object> { { key, value } }")]
        public static IDictionary<object, object> ObjectDictionary(object key, object value) {
            return new Dictionary<object, object> { { key, value } };
        }

        [ExpectedExpression("(a, b) => new SimpleClass { PropertyA = a, PropertyB = b }")]
        public static SimpleClass ClassWithProperties(object a, object b) {
            return new SimpleClass { PropertyA = a, PropertyB = b };
        }

        [ExpectedExpression("(a, b) => new SimpleClass { FieldA = a, FieldB = b }")]
        public static SimpleClass ClassWithFields(object a, object b) {
            return new SimpleClass { FieldA = a, FieldB = b };
        }

        [ExpectedExpression("(a, b) => new SimpleClass { PropertyA = a, Recursive = new SimpleClass { PropertyB = b } }")]
        public static SimpleClass ClassWithRecursiveUsingDirectSetter(object a, object b) {
            return new SimpleClass {
                PropertyA = a,
                Recursive = new SimpleClass { PropertyB = b }
            };
        }

        // not supported for now:
        // [ExpectedExpression("(a, b) => new SimpleClass { PropertyA = a, Recursive = { PropertyB = b }}")]
        public static SimpleClass ClassWithRecursiveUsingImplicitSetter(object a, object b) {
            return new SimpleClass {
                PropertyA = a,
                Recursive = { PropertyB = b }
            };
        }
    }
}
