using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expressive.Pipeline {
    public static class ListExtensions {
        public static int? IndexOf<T>(this IList<T> list, Func<T, bool> predicate) {
            for (var i = 0; i < list.Count; i++) {
                if (predicate(list[i]))
                    return i;
            }

            return null;
        }
    }
}
