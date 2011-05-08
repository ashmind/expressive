using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Elements {
    public interface IBranchingElement : IElement {
        IEnumerable<IList<IElement>> GetBranches();
    }
}
