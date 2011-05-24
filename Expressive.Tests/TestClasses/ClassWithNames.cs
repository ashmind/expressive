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

        [ExpectedExpression(@"{0} => String.Concat({0}.FirstName, "" "", {0}.LastName)")]
        public string FullNameSimple {
            get { return this.FirstName + " " + this.LastName; }
        }

        private const string FullNameExpression1 = @"{0} => (!String.IsNullOrEmpty({0}.FirstName) ? String.Concat({0}.FirstName, "" "", {0}.LastName) : {0}.LastName)";
        private const string FullNameExpression2 = @"{0} => (String.IsNullOrEmpty({0}.FirstName) ? {0}.LastName : String.Concat({0}.FirstName, "" "", {0}.LastName))";

        [ExpectedExpression(FullNameExpression1, FullNameExpression2)]
        public string FullNameWithInlineConditional {
            get {
                return string.IsNullOrEmpty(this.FirstName)
                     ? this.LastName
                     : this.FirstName + " " + this.LastName;
            }
        }

        [ExpectedExpression(FullNameExpression1, FullNameExpression2)]
        public string FullNameWithExplicitConditional {
            get {
                if (string.IsNullOrEmpty(this.FirstName))
                    return this.LastName;

                return this.FirstName + " " + this.LastName;
            }
        }
    }
}
