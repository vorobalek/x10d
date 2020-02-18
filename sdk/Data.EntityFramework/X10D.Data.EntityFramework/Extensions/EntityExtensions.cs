using System;
using System.Collections.Generic;
using X10D.Data.Entities.Abstractions;

namespace X10D.Data.EntityFramework
{
    /// <summary>
    /// Расширения для типов сущностей.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Возвращает имя таблицы в БД, которой сопоставлена сущность.
        /// </summary>
        /// <param name="type">Тип сущности.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Тип {type.GetFullName()} не должен быть интерфейсом. - type
        /// or
        /// Тип {type.GetFullName()} не должен быть абстрактным. - type
        /// or
        /// Тип {type.GetFullName()} не является производным от {typeof(IEmptyEntity).GetFullName()}. - type
        /// </exception>
        public static string GetTableName(this Type type)
        {
            if (type.IsInterface)
            {
                throw new ArgumentException($"Тип {type.GetFullName()} не должен быть интерфейсом.", nameof(type));
            }
            if (type.IsAbstract)
            {
                throw new ArgumentException($"Тип {type.GetFullName()} не должен быть абстрактным.", nameof(type));
            }
            if (!type.IsAssignableFrom(typeof(IEmptyEntity)))
            {
                throw new ArgumentException($"Тип {type.GetFullName()} не является производным от {typeof(IEmptyEntity).GetFullName()}.", nameof(type));
            }
            return type.Name.ToLowerInvariant();
        }

        /// <summary>
        /// Получить имя колонки первичного ключа сущности в БД.
        /// </summary>
        /// <typeparam name="TKey">Тип первичного ключа.</typeparam>
        /// <param name="type">Тип первичного ключа.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Тип {type.GetFullName()} не должен быть интерфейсом. - type
        /// or
        /// Тип {type.GetFullName()} не должен быть абстрактным. - type
        /// or
        /// Тип {type.GetFullName()} не является производным от {typeof(IPrimaryKeyEntity`<typeparamref name="TKey"/>).GetFullName()}. - type
        /// or
        /// Автоименование не доступно для первичных ключей, представленных generic-типами. {nameof(<typeparamref name="TKey"/>)} имеет тип {typeof(<typeparamref name="TKey"/>).GetFullName()}. - <typeparamref name="TKey"/>
        /// </exception>
        public static string GetPrimaryKeyName<TKey>(this Type type)
            where TKey : IComparable
        {
            if (type.IsInterface)
            {
                throw new ArgumentException($"Тип {type.GetFullName()} не должен быть интерфейсом.", nameof(type));
            }
            if (type.IsAbstract)
            {
                throw new ArgumentException($"Тип {type.GetFullName()} не должен быть абстрактным.", nameof(type));
            }
            if (!type.IsAssignableFrom(typeof(IPrimaryKeyEntity<TKey>)))
            {
                throw new ArgumentException($"Тип {type.GetFullName()} не является производным от {typeof(IPrimaryKeyEntity<TKey>).GetFullName()}.", nameof(type));
            }
            if (typeof(TKey).IsGenericType)
            {
                throw new ArgumentException($"Автоименование не доступно для первичных ключей, представленных generic-типами. {nameof(TKey)} имеет тип {typeof(TKey).GetFullName()}.", nameof(TKey));
            }
            return $"{type.Name}id".ToLowerInvariant();
        }

        /// <summary>
        /// Возвращает имя таблицы в БД, которой сопоставлена сущность.
        /// </summary>
        /// <param name="entity">Экземпляр сущности.</param>
        /// <returns></returns>
        public static string GetTableName(this IEmptyEntity entity)
        {
            return entity.GetType().GetTableName();
        }

        /// <summary>
        /// Получить имя колонки первичного ключа сущности в БД.
        /// </summary>
        /// <typeparam name="TKey">Тип первичного ключа.</typeparam>
        /// <param name="entity">Экземпляр сущности.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Тип {type.GetFullName()} не должен быть интерфейсом. - type
        /// or
        /// Тип {type.GetFullName()} не должен быть абстрактным. - type
        /// or
        /// Тип {type.GetFullName()} не является производным от {typeof(IPrimaryKeyEntity{TKey}).GetFullName()}. - type
        /// or
        /// Автоименование не доступно для первичных ключей, представленных generic-типами. {nameof(<typeparamref name="TKey"/>)} имеет тип {typeof(<typeparamref name="TKey"/>).GetFullName()}. - <typeparamref name="TKey"/>
        /// </exception>
        public static string GetPrimaryKeyName<TKey>(this IPrimaryKeyEntity<TKey> entity)
            where TKey : IComparable
        {
            if (typeof(TKey).IsGenericType)
            {
                throw new ArgumentException($"Автоименование не доступно для первичных ключей, представленных generic-типами. {nameof(TKey)} имеет тип {typeof(TKey).GetFullName()}", nameof(TKey));
            }
            return entity.GetType().GetPrimaryKeyName<TKey>();
        }

        /// <summary>
        /// Gets the original.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IDuplicatedEntity<TKey> GetOriginal<TKey>(this IDuplicatedEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the duplicates.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<IDuplicatedEntity<TKey>> GetDuplicates<TKey>(this IDuplicatedEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the higher.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IHierarchicalEntity<TKey> GetHigher<TKey>(this IHierarchicalEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the inferior.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IHierarchicalEntity<TKey> GetInferior<TKey>(this IHierarchicalEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the inferiors.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<IHierarchicalEntity<TKey>> GetInferiors<TKey>(this IHierarchicalEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IVersionedEntity<TKey> GetParent<TKey>(this IVersionedEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IVersionedEntity<TKey> GetChild<TKey>(this IVersionedEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the childrens.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<IVersionedEntity<TKey>> GetChildrens<TKey>(this IVersionedEntity<TKey> entity)
            where TKey : struct, IComparable
        {
            throw new NotImplementedException();
        }
    }
}
