using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class MemberAssignmentElement : IElement {
        public Expression Instance { get; set; }
        public MemberInfo Member { get; private set; }
        public Expression Value  { get; set; }

        public MemberAssignmentElement(Expression instance, MemberInfo member, Expression value) {
            this.Instance = instance;
            this.Member = member;
            this.Value = value;
        }

        public ElementKind Kind {
            get { return ElementKind.Statement; }
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var target = this.Instance != null
                       ? this.Instance.ToString()
                       : this.Member.DeclaringType.FullName;

            return string.Format(
                "{0}{1}.{2} = {3}",
                    indent.Value, target, this.Member.Name, this.Value
            );
        }
    }
}
