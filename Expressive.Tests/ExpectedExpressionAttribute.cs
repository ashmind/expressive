using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AshMind.Extensions;

namespace Expressive.Tests {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field, AllowMultiple = false)]
    public class ExpectedExpressionAttribute : Attribute {
        public ExpectedExpressionAttribute(params string[] patterns) {
            this.Patterns = patterns.AsReadOnly();
        }

        public ReadOnlyCollection<string> Patterns { get; private set; }
    }
}
