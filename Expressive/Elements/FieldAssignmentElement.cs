using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class FieldAssignmentElement : IElement {
        public Expression Instance { get; private set; }
        public FieldInfo Field { get; private set; }
        public Expression Value  { get; private set; }

        public FieldAssignmentElement(Expression instance, FieldInfo field, Expression value) {
            this.Instance = instance;
            this.Field = field;
            this.Value = value;
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var target = this.Instance != null
                       ? this.Instance.ToString()
                       : this.Field.DeclaringType.FullName;

            return string.Format(
                "{0}{1}.{2} = {3}",
                    indent.Value, target, this.Field.Name, this.Value
            );
        }
    }
}
