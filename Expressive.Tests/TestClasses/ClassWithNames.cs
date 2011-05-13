using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public class ClassWithNames {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [ExpectedExpression(@"() => ""Test""")]
        public static string StaticName {
            get { return "Test"; }
        }

        [ExpectedExpression("{0} => {0}.FirstName")]
        public string JustFirstName {
            get { return this.FirstName; }
        }

        [ExpectedExpression(@"{0} => Concat({0}.FirstName, "" "", {0}.LastName)")]
        public string FullNameSimple {
            get { return this.FirstName + " " + this.LastName; }
        }

        public string FullNameWithInlineConditional {
            get {
                return string.IsNullOrEmpty(this.FirstName)
                     ? this.LastName
                     : this.FirstName + " " + this.LastName;
            }
        }

        public string FullNameWithExplicitConditional {
            get {
                if (string.IsNullOrEmpty(this.FirstName))
                    return this.LastName;

                return this.FirstName + " " + this.LastName;
            }
        }
    }
}
