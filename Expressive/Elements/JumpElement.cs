using System.Collections.Generic;
using System.Linq;

namespace Expressive.Elements {
    public class JumpElement : IBranchingElement {
        public IList<IElement> FollowingBranch { get; private set; }
        public IList<IElement> TargetBranch { get; private set; }

        public JumpElement(IList<IElement> followingBranch, IList<IElement> targetBranch) {
            this.FollowingBranch = followingBranch;
            this.TargetBranch = targetBranch;
        }

        public override string ToString() {
            return "jump";
        }

        public IEnumerable<IList<IElement>> GetBranches() {
            yield return this.FollowingBranch;
            yield return this.TargetBranch;
        }
    }
}