using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public class ConditionalJumpElement : IBranchingElement {
        public IList<IElement> FollowingBranch { get; private set; }
        public IList<IElement> TargetBranch { get; private set; }
        public bool JumpWhenTrue { get; private set; }

        public ConditionalJumpElement(IList<IElement> followingBranch, IList<IElement> targetBranch, bool jumpWhenTrue) {
            this.FollowingBranch = followingBranch;
            this.TargetBranch = targetBranch;
            this.JumpWhenTrue = jumpWhenTrue;
        }

        public ConditionalJumpElement(IList<IElement> followingBranch, bool jumpWhenTrue)
            : this(followingBranch, new List<IElement>(), jumpWhenTrue)
        {
        }
        
        public override string ToString() {
            return this.ToString(Indent.FourSpaces);
        }

        public string ToString(Indent indent) {
            var builder = new StringBuilder();
            builder.Append(indent.Value)
                   .Append("if it was ")
                   .Append(this.JumpWhenTrue ? "false" : "true")
                   .Append(" do")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.FollowingBranch, indent.Increase()));

            if (this.TargetBranch.Count == 0)
                return builder.ToString();

            builder.AppendLine()
                   .Append(indent.Value)
                   .Append("otherwise")
                   .AppendLine()
                   .Append(ElementHelper.ToString(this.TargetBranch, indent.Increase()));
            return builder.ToString();
        }

        public IEnumerable<IList<IElement>> GetBranches() {
            yield return this.FollowingBranch;
            yield return this.TargetBranch;
        }
    }
}
