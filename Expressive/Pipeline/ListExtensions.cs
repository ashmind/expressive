using System;
using System.Collections.Generic;
using System.Linq;

namespace Expressive.Pipeline {
    public static class ListExtensions {
        public static int? IndexOf<T>(this IList<T> list, Func<T, bool> predicate) {
            return list.IndexOf((item, i) => predicate(item));
        }

        public static int? IndexOf<T>(this IList<T> list, Func<T, int, bool> predicate) {
            for (var i = 0; i < list.Count; i++) {
                if (predicate(list[i], i))
                    return i;
            }

            return null;
        }
    }
}
