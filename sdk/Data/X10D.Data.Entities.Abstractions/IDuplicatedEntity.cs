using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс сущности, у которой могут быть дублирующие экземпляры.
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа сущности.</typeparam>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IPrimaryKeyEntity{TKey}" />
    public interface IDuplicatedEntity<TKey> : IPrimaryKeyEntity<TKey>
        where TKey : struct, IComparable
    {
        /// <summary>
        /// Значение первичного ключа оригинального экземпляра сущности.
        /// </summary>
        TKey? OriginalId { get; set; }
    }
}