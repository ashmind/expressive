using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public class ComplexClass {
        public static readonly int Int32Field = 1000;
        public int Int32Property { get; set; }
        public bool BooleanProperty { get; set; }

        public bool ComplexProperty1 {
            get {
                return this.Int32Property > Int32Field
                    && this.BooleanProperty;
            }
        }

        public bool ComplexProperty2 {
            get {
                return this.Int32Property > 0
                    || !(this.BooleanProperty);
            }
        }
    }
}
