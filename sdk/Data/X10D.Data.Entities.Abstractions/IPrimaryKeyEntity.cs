using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс сущности с первичным ключом.
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа сущности.</typeparam>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IEmptyEntity" />
    public interface IPrimaryKeyEntity<TKey> : IEmptyEntity
        where TKey : IComparable
    {
        /// <summary>
        /// Значение первичного ключа.
        /// </summary>
        TKey Id { get; set; }
    }
}