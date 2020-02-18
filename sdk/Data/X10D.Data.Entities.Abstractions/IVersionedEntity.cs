using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс сущности, поддерживающей версионную структуру экземпляров.
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа сущности.</typeparam>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IPrimaryKeyEntity{TKey}" />
    public interface IVersionedEntity<TKey> : IPrimaryKeyEntity<TKey>
        where TKey : struct, IComparable
    {
        /// <summary>
        /// Значение первичного ключа экземпляра родительской сущности.
        /// </summary>
        TKey? ParentId { get; set; }
    }
}