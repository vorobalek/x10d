using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс производной базовой сущности.
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа базовой сущности.</typeparam>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IPrimaryKeyEntity{TKey}" />
    /// <seealso cref="X10D.Data.Entities.Abstractions.IDatedEntity" />
    public interface IBaseEntity<TKey> : IPrimaryKeyEntity<TKey>, IDatedEntity
        where TKey : IComparable
    {
    }
}