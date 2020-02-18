using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Расширения для использования рефлексии.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Выражение не ссылается на доступный элемент типа {typeof(T).GetFullName()} - expression</exception>
        public static string GetPropertyName<T, U>(Expression<Func<T, U>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }
            throw new ArgumentException($"Выражение не ссылается на доступный элемент типа {typeof(T).GetFullName()}", nameof(expression));
        }
    }
}
