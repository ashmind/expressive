using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Elements.Presentation {
    public class Indent {
        public static Indent None { get; private set; }
        public static Indent FourSpaces { get; private set; }

        private readonly Func<string, string> increase;
        public string Value { get; private set; }

        static Indent() {
            None = new Indent("", _ => _);
            FourSpaces = new Indent("", s => s + "    ");
        }

        public Indent(string value, Func<string, string> increase) {
            this.increase = increase;
            this.Value = value;
        }

        public Indent Increase() {
            return new Indent(this.increase(this.Value), this.increase);
        }
    }
}
