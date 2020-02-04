using System;
using System.Collections.Generic;
using System.Linq;

namespace HY_PML.helper
{
    public static class LinqExt
    {
        public static IOrderedEnumerable<T> NullableOrderBy<T>(this IEnumerable<T> list, Func<T, string> keySelector)
        {
            return list.OrderBy(v => keySelector(v) != null ? 0 : 1).ThenBy(keySelector);
        }

        public static IOrderedEnumerable<T> NullableOrderByDescending<T>(this IEnumerable<T> list, Func<T, string> keySelector)
        {
            return list.OrderByDescending(v => keySelector(v) != null ? 0 : 1).ThenBy(keySelector);
        }
    }
}