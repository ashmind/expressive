using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Elements.Presentation;

namespace Expressive.Elements {
    public interface IElement {
        string ToString(Indent indent);
        ElementKind Kind { get; }
    }
}
