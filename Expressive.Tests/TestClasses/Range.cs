using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expressive.Tests.TestClasses {
    // http://stackoverflow.com/questions/3916122/nhibernate-3-extending-linq-provider-basehqlgeneratorformethod-buildhql-problem
    public class Range {
        public int Min { get; set; }
        public int Max { get; set; }

        [ExpectedExpression("({0}, value) => ((value >= {0}.Min) AndAlso (value <= {0}.Max))")]
        public bool Contains(int value) {
            return value >= this.Min && value <= this.Max;
        }
    }
}
