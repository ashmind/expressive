using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Elements.Presentation {
    public static class ElementHelper {
        public static string ToString(IEnumerable<IElement> elements, Indent indent) {
            return string.Join(
                Environment.NewLine,
                elements.Select(e => e.ToString(indent)).ToArray()
            );
        }
    }
}
