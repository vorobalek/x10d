using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс иерархической сущности.
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа сущности.</typeparam>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IPrimaryKeyEntity{TKey}" />
    public interface IHierarchicalEntity<TKey> : IPrimaryKeyEntity<TKey>
        where TKey : struct, IComparable
    {
        /// <summary>
        /// Значение первичного ключа вышестоящего по иерархии экземпляра сущности.
        /// </summary>
        TKey? HigherId { get; set; }
    }
}