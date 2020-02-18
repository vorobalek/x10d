namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс базовой сущности, отображаемой в БД.
    /// </summary>
    /// <seealso cref="ExtCore.Data.Entities.Abstractions.IEntity" />
    public interface IEmptyEntity : ExtCore.Data.Entities.Abstractions.IEntity
    {
        /// <summary>
        /// Признак того, что сущность удалена.
        /// (Операция удаления в системе означает установку флага <c>IsDeleted = true</c>, а не полное физическое удаление записи из БД.)
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        bool IsDeleted { get; set; }
    }
}