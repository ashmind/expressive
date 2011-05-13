using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Tests.TestClasses {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
    public class ExpectedExpressionAttribute : Attribute {
        public ExpectedExpressionAttribute(string pattern) {
            this.Pattern = pattern;
        }

        public string Pattern { get; private set; }
    }
}
