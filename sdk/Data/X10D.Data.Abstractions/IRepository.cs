using System;
using System.Collections.Generic;

namespace X10D.Data.Abstractions
{
    /// <summary>
    /// Типизируемый интерфейс слоя доступа к данным.
    /// </summary>
    /// <typeparam name="TEntity">Тип целевой сущности.</typeparam>
    public interface IRepository<TEntity> : ExtCore.Data.Abstractions.IRepository
    {
        /// <summary>
        /// Тип сущности, на работу с которым нацелена реализация типа.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Пробует выполнить "сырой" SQL запрос к базе данных, который не должен возвращать значений.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <param name="args">Набор параметров запроса. Необходимо использовать <c>SQLParameter</c></param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryExecuteNonQuery(string query, out Exception exception, params object[] args);

        /// <summary>
        /// Пробует выполнить "сырой" SQL запрос к базе данных, и преобразовать его результат к набору <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="query">Запрос.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <param name="results">Набор <typeparamref name="TResult"/>, сформированный из результата выполнения запроса, если есть.</param>
        /// <param name="args">Набор параметров запроса. Необходимо использовать <c>SQLParameter</c></param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryExecuteQuery<TResult>(string query, out Exception exception, out IEnumerable<TResult> results, params object[] args)
            where TResult : class;

        /// <summary>
        /// Пробует автоматически создать сущность в БД.
        /// </summary>
        /// <param name="entity">Данные для заполнения новой сущности.</param>
        /// <param name="result">Экземпляр созданной сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryCreate(TEntity entity, out TEntity result, out Exception exception);

        /// <summary>
        /// Пробует автоматически создать набор сущностей в БД.
        /// </summary>
        /// <param name="entities">Данные для заполнения новых сущностей</param>
        /// <param name="results">Экземпляры созданных сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryCreate(IEnumerable<TEntity> entities, out IEnumerable<TEntity> results, out Exception exception);

        /// <summary>
        /// Пробует автоматически получить сущность из БД, удовлетворяющих условию.
        /// </summary>
        /// <param name="predicate">Условие поиска сущнсоти.</param>
        /// <param name="result">Экземпляр найденной сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryRead(Func<TEntity, bool> predicate, out TEntity result, out Exception exception);

        /// <summary>
        /// Пробует автоматически получить набор сущностей из БД, удовлетворяющих условию.
        /// </summary>
        /// <param name="predicate">Условие поиска сущнсотей.</param>
        /// <param name="results">Экземпляры найденных сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryRead(Func<TEntity, bool> predicate, out IEnumerable<TEntity> results, out Exception exception);

        /// <summary>
        /// Пробует автоматически обновить сущность в БД.
        /// </summary>
        /// <param name="entity">Новые данные для заполнения сущности.</param>
        /// <param name="result">Экземпляр обновлённой сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryUpdate(TEntity entity, out TEntity result, out Exception exception);

        /// <summary>
        /// Пробует автоматически обновить набор сущностей в БД.
        /// </summary>
        /// <param name="entities">Новые данные для заполнения сущностей.</param>
        /// <param name="results">Экземпляры обновлённых сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryUpdate(IEnumerable<TEntity> entities, out IEnumerable<TEntity> results, out Exception exception);

        /// <summary>
        /// Пробует автоматически удалить сущность в БД.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="entity">Экземпляр целевой сущности.</param>
        /// <param name="result">Экземпляр удалённой сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryDelete(TEntity entity, out TEntity result, out Exception exception);

        /// <summary>
        /// Пробует автоматически удалить набор сущностей в БД.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="entities">Экземпляры целевых сущностей.</param>
        /// <param name="results">Экземпляры удалённых сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryDelete(IEnumerable<TEntity> entities, out IEnumerable<TEntity> results, out Exception exception);
    }

    /// <summary>
    /// Типизируемый интерфейс слоя доступа к данным сущностей с первичным ключом.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
    {
        /// <summary>
        /// Тип первичного ключа сущности.
        /// </summary>
        Type EntityIdType { get; }

        /// <summary>
        /// Пробует автоматически получить сущность из БД по первичному ключу.
        /// </summary>
        /// <param name="key">Значение первичного ключа.</param>
        /// <param name="result">Экземпляр найденной сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryRead(TKey key, out TEntity result, out Exception exception);

        /// <summary>
        /// Пробует автоматически получить набор сущностей из БД по набору первичных ключей.
        /// </summary>
        /// <param name="keys">Набор значений первичных ключей.</param>
        /// <param name="results">Экземпляры найденных сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryRead(IEnumerable<TKey> keys, out IEnumerable<TEntity> results, out Exception exception);

        /// <summary>
        /// Пробует автоматически удалить сущность из БД по первичному ключу.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="key">Значение первичного ключа.</param>
        /// <param name="result">Экземпляр удалённой сущности, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryDelete(TKey key, out TEntity result, out Exception exception);

        /// <summary>
        /// Пробует автоматически удалить набор сущностей из БД по набору первичных ключей.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <param name="keys">Набор значений первичных ключей.</param>
        /// <param name="results">Экземпляры удалённых сущностей, если есть.</param>
        /// <param name="exception">Ошибка при выполнении операции, если есть.</param>
        /// <returns>Признак успеха выполнения операции.</returns>
        bool TryDelete(IEnumerable<TKey> keys, out IEnumerable<TEntity> results, out Exception exception);
    }
}
