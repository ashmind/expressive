﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    public class ClassWithNames {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static string StaticName {
            get { return "Test"; }
        }

        public string JustFirstName {
            get { return this.FirstName; }
        }

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
