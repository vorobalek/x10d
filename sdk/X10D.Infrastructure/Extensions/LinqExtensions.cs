using System.Collections.Generic;

namespace System.Linq
{
    /// <summary>
    /// Расширения коллекций.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Does the action.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="outer">The outer.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DoAction<TSource>(this IEnumerable<TSource> outer, Action<TSource> action)
        {
            foreach (var item in outer)
            {
                action(item);
            }

            return outer;
        }
    }
}
