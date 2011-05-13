using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public class BooleanOperations {
        public static readonly int Int32Field = 1000;
        public int Int32Property { get; set; }
        public bool BooleanProperty { get; set; }

        [ExpectedExpression("{0} => (({0}.Int32Property > BooleanOperations.Int32Field) AndAlso {0}.BooleanProperty)")]
        public bool ComplexProperty1 {
            get {
                return this.Int32Property > Int32Field
                    && this.BooleanProperty;
            }
        }

        [ExpectedExpression("{0} => (({0}.Int32Property > 0) OrElse Not({0}.BooleanProperty))")]
        public bool ComplexProperty2 {
            get {
                return this.Int32Property > 0
                    || !(this.BooleanProperty);
            }
        }

        [ExpectedExpression("(a, b) => ((a == b) OrElse ((a != null) AndAlso ((b != null) AndAlso a.Equals(b))))")]
        public static bool ObjectEquals(object a, object b) {
            return a == b
                || (
                    (a != null) && (b != null) && a.Equals(b)
                );
        }
    }
}
