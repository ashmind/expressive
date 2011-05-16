using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class IfThenElement : IElement {
        public Expression Condition { get; set; }
        public IList<IElement> Then { get; set; }
        public IList<IElement> Else { get; set; }

        public IfThenElement(Expression condition, IList<IElement> then, IList<IElement> @else) {
            if (condition.Type != typeof(bool))
                throw new ArgumentException("Expected condition to be boolean.", "condition");

            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
        }
        
        public IEnumerable<IList<IElement>> GetBranches() {
            yield return this.Then;
            yield return this.Else;
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var builder = new StringBuilder();
            builder.Append(indent.Value)
                   .Append("if ")
                   .Append(this.Condition)
                   .Append(" then:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.Then, indent.Increment()));

            if (this.Else.Count == 0)
                return builder.ToString();

            builder.AppendLine()
                   .Append(indent.Value)
                   .Append("else:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.Else, indent.Increment()));
            return builder.ToString();
        }
    }
}
