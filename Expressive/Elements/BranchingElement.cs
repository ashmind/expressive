using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using AshMind.Extensions;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class BranchingElement : IElement {      
        private static readonly IDictionary<string, int> nullaryAndUnaryParameterCounts = new Dictionary<string, int> {
            { OpCodes.Br.Name, 0 },
            { OpCodes.Brtrue.Name, 1 },
            { OpCodes.Brfalse.Name, 1 }
        };

        public OpCode OpCode  { get; set; }
        public IList<IElement> Target  { get; private set; }
        public IList<IElement> Fallback { get; private set; }

        public int ParameterCount {
            get { return nullaryAndUnaryParameterCounts.GetValueOrDefault(this.OpCode.Name.SubstringBefore("."), 2); }
        }

        public BranchingElement(OpCode opCode, IList<IElement> target, IList<IElement> fallback) {
            this.OpCode = opCode;
            this.Target = target;
            this.Fallback = fallback;
        }

        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var builder = new StringBuilder();
            var innerIndent = indent.Increment();
            builder.Append(indent.Value)
                   .AppendFormat("branch ({0}):", this.OpCode)
                   .AppendLine()
                   .Append(innerIndent.Value)
                   .Append("target:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.Target, innerIndent.Increment()));
            
            builder.AppendLine()
                   .Append(innerIndent.Value)
                   .Append("fallback:")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.Fallback, innerIndent.Increment()));
            return builder.ToString();
        }

        public IEnumerable<IList<IElement>> GetBranches() {
            yield return this.Target;
            yield return this.Fallback;
        }
    }
}
