using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using X10D.Data.Abstractions;
using X10D.Data.Entities.Abstractions;

namespace X10D.Data.EntityFramework
{
    /// <summary>
    /// Типизируемый абстрактный класс слоя доступа к данным средствами EntityFramework.
    /// </summary>
    /// <typeparam name="TEntity">Тип целевой сущности.</typeparam>
    public abstract class Repository<TEntity> : ExtCore.Data.EntityFramework.RepositoryBase<TEntity>, IRepository<TEntity>
        where TEntity : class, IEmptyEntity
    {
        /// <summary>
        /// Тип сущности, на работу с которым нацелена реализация типа.
        /// </summary>
        public Type EntityType => typeof(TEntity);

        /// <summary>
        /// Пробует автоматически создать сущность в БД.
        /// </summary>
        /// <param name="entity">Данные для заполнения новой сущности.</param>
        /// <param name="result">Экземпляр созданной сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryCreate(TEntity entity, out TEntity result, out Exception exception)
        {
            result = null;
            exception = null;
            try
            {
                result = Create(entity);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически создать набор сущностей в БД.
        /// </summary>
        /// <param name="entities">Данные для заполнения новых сущностей</param>
        /// <param name="results">Экземпляры созданных сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryCreate(IEnumerable<TEntity> entities, out IEnumerable<TEntity> results, out Exception exception)
        {
            results = null;
            exception = null;
            try
            {
                results = Create(entities);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует выполнить "сырой" SQL запрос к базе данных, который не должен возвращать значений.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <param name="args">Набор параметров запроса. Необходимо использовать <c>SQLParameter</c></param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryExecuteNonQuery(string query, out Exception exception, params object[] args)
        {
            exception = null;
            try
            {
                ExecuteNonQuery(query, args);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует выполнить "сырой" SQL запрос к базе данных, и преобразовать его результат к набору <typeparamref name="TResult" />.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">Запрос.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <param name="results">Набор <typeparamref name="TResult" />, сформированный из результата выполнения запроса, если есть.</param>
        /// <param name="args">Набор параметров запроса. Необходимо использовать <c>SQLParameter</c></param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryExecuteQuery<TResult>(string query, out Exception exception, out IEnumerable<TResult> results, params object[] args)
            where TResult : class
        {
            results = null;
            exception = null;
            try
            {
                results = ExecuteQuery<TResult>(query, args);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически получить сущность из БД, удовлетворяющих условию.
        /// </summary>
        /// <param name="predicate">Условие поиска сущнсоти.</param>
        /// <param name="result">Экземпляр найденной сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryRead(Func<TEntity, bool> predicate, out TEntity result, out Exception exception)
        {
            result = null;
            exception = null;
            try
            {
                result = Read(predicate).FirstOrDefault();
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически получить набор сущностей из БД, удовлетворяющих условию.
        /// </summary>
        /// <param name="predicate">Условие поиска сущнсотей.</param>
        /// <param name="results">Экземпляры найденных сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryRead(Func<TEntity, bool> predicate, out IEnumerable<TEntity> results, out Exception exception)
        {
            results = null;
            exception = null;
            try
            {
                results = Read(predicate);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически обновить сущность в БД.
        /// </summary>
        /// <param name="entity">Новые данные для заполнения сущности.</param>
        /// <param name="result">Экземпляр обновлённой сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryUpdate(TEntity entity, out TEntity result, out Exception exception)
        {
            result = null;
            exception = null;
            try
            {
                result = Update(entity);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически обновить набор сущностей в БД.
        /// </summary>
        /// <param name="entities">Новые данные для заполнения сущностей.</param>
        /// <param name="results">Экземпляры обновлённых сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryUpdate(IEnumerable<TEntity> entities, out IEnumerable<TEntity> results, out Exception exception)
        {
            results = null;
            exception = null;
            try
            {
                results = Update(entities);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически удалить сущность в БД.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="entity">Экземпляр целевой сущности.</param>
        /// <param name="result">Экземпляр удалённой сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryDelete(TEntity entity, out TEntity result, out Exception exception)
        {
            result = null;
            exception = null;
            try
            {
                result = Delete(entity);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически удалить набор сущностей в БД.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="entities">Экземпляры целевых сущностей.</param>
        /// <param name="results">Экземпляры удалённых сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryDelete(IEnumerable<TEntity> entities, out IEnumerable<TEntity> results, out Exception exception)
        {
            results = null;
            exception = null;
            try
            {
                results = Delete(entities);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Выполнить "сырой" SQL запрос к базе данных, который не должен возвращать значений.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <param name="args">Набор параметров запроса. Необходимо использовать <c>SQLParameter</c></param>
        protected void ExecuteNonQuery(string query, params object[] args)
        {
            storageContext.Database.ExecuteSqlRaw(query, args);
        }

        /// <summary>
        /// Выполнить "сырой" SQL запрос к базе данных, и преобразовать его результат к набору <typeparamref name="TResult" />.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query">Запрос.</param>
        /// <param name="args">Набор параметров запроса. Необходимо использовать <c>SQLParameter</c></param>
        /// <returns>
        /// Набор <typeparamref name="TResult" />, сформированный из результата выполнения запроса
        /// </returns>
        protected IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] args)
            where TResult : class
        {
            return storageContext.Set<TResult>().FromSqlRaw<TResult>(query, args);
        }

        /// <summary>
        /// Автоматически создать сущность в БД.
        /// </summary>
        /// <param name="entity">Данные для заполнения новой сущности.</param>
        /// <returns>
        /// Экземпляр созданной сущности.
        /// </returns>
        protected virtual TEntity Create(TEntity entity)
        {
            var entities = new[] { entity };
            return Create(entities).FirstOrDefault();
        }

        /// <summary>
        /// Автоматически создать набор сущностей в БД.
        /// </summary>
        /// <param name="entities">Данные для заполнения новых сущностей</param>
        /// <returns>
        /// Экземпляры созданных сущностей.
        /// </returns>
        protected virtual IEnumerable<TEntity> Create(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0)
            {
                throw new ArgumentException($"Коллекция не может быть пустой!", nameof(entities));
            }

            var typesMatch = entities.All(e => e.GetType() == EntityType);
            if (!typesMatch)
            {
                throw new ArgumentException($"Исходный набор содержит сущности недопустимых типов данных. " +
                    $"Эта операция требует, чтобы все сущности набора входных данных принадлежали типу ${EntityType.GetFullName()}", nameof(entities));
            }

            dbSet.AddRange(entities);
            return entities;
        }

        /// <summary>
        /// Автоматически получить сущность из БД, удовлетворяющих условию.
        /// </summary>
        /// <param name="predicate">Условие поиска сущнсоти.</param>
        /// <returns>
        /// Экземпляр найденной сущности.
        /// </returns>
        protected virtual IEnumerable<TEntity> Read(Func<TEntity, bool> predicate)
        {
            return dbSet.Where(predicate);
        }

        /// <summary>
        /// Автоматически обновить сущность в БД.
        /// </summary>
        /// <param name="entity">Новые данные для заполнения сущности.</param>
        /// <returns>
        /// Экземпляр обновлённой сущности.
        /// </returns>
        protected virtual TEntity Update(TEntity entity)
        {
            var entities = new[] { entity };
            return Update(entities).FirstOrDefault();
        }

        /// <summary>
        /// Автоматически обновить набор сущностей в БД.
        /// </summary>
        /// <param name="entities">Новые данные для заполнения сущностей.</param>
        /// <returns>
        /// Экземпляры обновлённых сущностей.
        /// </returns>
        protected virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0)
            {
                throw new ArgumentException($"Коллекция не может быть пустой!", nameof(entities));
            }

            var typesMatch = entities.All(e => e.GetType() == (this as IRepository<TEntity>).EntityType);
            if (!typesMatch)
            {
                throw new ArgumentException($"Исходный набор содержит сущности недопустимых типов данных. " +
                    $"Эта операция требует, чтобы все сущности набора входных данных принадлежали типу ${(this as IRepository<TEntity>).EntityType.GetFullName()}", nameof(entities));
            }

            dbSet.UpdateRange(entities);
            return entities;
        }

        /// <summary>
        /// Автоматически удалить сущность в БД.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="entity">Экземпляр целевой сущности.</param>
        /// <returns>
        /// Экземпляр удалённой сущности.
        /// </returns>
        protected virtual TEntity Delete(TEntity entity)
        {
            var entities = new[] { entity };
            return Delete(entities).FirstOrDefault();
        }

        /// <summary>
        /// Автоматически удалить набор сущностей в БД.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="entities">Экземпляры целевых сущностей.</param>
        /// <returns>
        /// Экземпляры удалённых сущностей.
        /// </returns>
        protected virtual IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities)
        {
            if (entities.Count() == 0)
            {
                throw new ArgumentException($"Коллекция не может быть пустой!", nameof(entities));
            }

            var typesMatch = entities.All(e => e.GetType() == (this as IRepository<TEntity>).EntityType);
            if (!typesMatch)
            {
                throw new ArgumentException($"Исходный набор содержит сущности недопустимых типов данных. " +
                    $"Эта операция требует, чтобы все сущности набора входных данных принадлежали типу ${(this as IRepository<TEntity>).EntityType.GetFullName()}", nameof(entities));
            }

            return Update(entities.DoAction(e => e.IsDeleted = true));
        }
    }

    /// <summary>
    /// Типизируемый aбстрактный класс слоя доступа к данным средствами EntityFramework сущностей с первичным ключом.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Reposityory<TEntity, TKey> : Repository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class, IPrimaryKeyEntity<TKey>
        where TKey : IComparable
    {
        /// <summary>
        /// Тип первичного ключа сущности.
        /// </summary>
        public Type EntityIdType => typeof(TKey);

        /// <summary>
        /// Пробует автоматически получить сущность из БД по первичному ключу.
        /// </summary>
        /// <param name="key">Значение первичного ключа.</param>
        /// <param name="result">Экземпляр найденной сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryRead(TKey key, out TEntity result, out Exception exception)
        {
            result = null;
            exception = null;
            try
            {
                result = Read(key);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически получить набор сущностей из БД по набору первичных ключей.
        /// </summary>
        /// <param name="keys">Набор значений первичных ключей.</param>
        /// <param name="results">Экземпляры найденных сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryRead(IEnumerable<TKey> keys, out IEnumerable<TEntity> results, out Exception exception)
        {
            results = null;
            exception = null;
            try
            {
                results = Read(keys);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически удалить сущность из БД по первичному ключу.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="key">Значение первичного ключа.</param>
        /// <param name="result">Экземпляр удалённой сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryDelete(TKey key, out TEntity result, out Exception exception)
        {
            result = null;
            exception = null;
            try
            {
                result = Delete(key);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Пробует автоматически удалить набор сущностей из БД по набору первичных ключей.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="keys">Набор значений первичных ключей.</param>
        /// <param name="results">Экземпляры удалённых сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>
        /// Признак успеха выполнения операции.
        /// </returns>
        public bool TryDelete(IEnumerable<TKey> keys, out IEnumerable<TEntity> results, out Exception exception)
        {
            results = null;
            exception = null;
            try
            {
                results = Delete(keys);
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Автоматически получить сущность из БД по первичному ключу.
        /// </summary>
        /// <param name="key">Значение первичного ключа.</param>
        /// <returns>
        /// Экземпляр найденной сущности.
        /// </returns>
        protected virtual TEntity Read(TKey key)
        {
            var keys = new[] { key };
            return Read(keys).FirstOrDefault();
        }

        /// <summary>
        /// Автоматически получить набор сущностей из БД по набору первичных ключей.
        /// </summary>
        /// <param name="keys">Набор значений первичных ключей.</param>
        /// <returns>
        /// Экземпляры найденных сущностей.
        /// </returns>
        protected virtual IEnumerable<TEntity> Read(IEnumerable<TKey> keys)
        {
            if (keys.Count() == 0)
            {
                throw new ArgumentException($"Коллекция не может быть пустой!", nameof(keys));
            }

            var typesMatch = keys.All(e => e.GetType() == EntityIdType);
            if (!typesMatch)
            {
                throw new ArgumentException($"Исходный набор содержит сущности недопустимых типов данных. " +
                    $"Эта операция требует, чтобы все сущности набора входных данных принадлежали типу ${EntityIdType.GetFullName()}", nameof(keys));
            }
            var keys_list = keys.ToList();
            return dbSet.Where(e => keys_list.Contains(e.Id));
        }

        /// <summary>
        /// Автоматически удалить сущность из БД по первичному ключу.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="key">Значение первичного ключа.</param>
        /// <returns>
        /// Экземпляр удалённой сущности.
        /// </returns>
        protected virtual TEntity Delete(TKey key)
        {
            var keys = new[] { key };
            return Delete(keys).FirstOrDefault();
        }

        /// <summary>
        /// Автоматически удалить набор сущностей из БД по набору первичных ключей.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="keys">Набор значений первичных ключей.</param>
        /// <returns>
        /// Экземпляры удалённых сущностей.
        /// </returns>
        protected virtual IEnumerable<TEntity> Delete(IEnumerable<TKey> keys)
        {
            return Delete(Read(keys));
        }
    }
}
