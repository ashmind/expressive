using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public class ClassWithNames {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullNameSimple {
            get { return this.FirstName + " " + this.LastName; }
        }
    }
}
