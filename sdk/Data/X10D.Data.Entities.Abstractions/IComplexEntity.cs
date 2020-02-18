using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс производной комплексной сущности.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IDuplicatedEntity{TKey}" />
    /// <seealso cref="X10D.Data.Entities.Abstractions.IHierarchicalEntity{TKey}" />
    /// <seealso cref="X10D.Data.Entities.Abstractions.IVersionedEntity{TKey}" />
    public interface IComplexEntity<TKey> : IDuplicatedEntity<TKey>, IHierarchicalEntity<TKey>, IVersionedEntity<TKey>
        where TKey: struct, IComparable
    {
    }
}