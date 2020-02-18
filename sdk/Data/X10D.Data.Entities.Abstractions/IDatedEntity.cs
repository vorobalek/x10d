using System;

namespace X10D.Data.Entities.Abstractions
{
    /// <summary>
    /// Интерфейс сущности, отслеживающей дату создания и/или изменения.
    /// </summary>
    /// <seealso cref="X10D.Data.Entities.Abstractions.IEmptyEntity" />
    public interface IDatedEntity : IEmptyEntity
    {
        /// <summary>
        /// Дата создания.
        /// </summary>
        DateTime CreationDate { get; set; }

        /// <summary>
        /// Дата изменения.
        /// </summary>
        DateTime ModifiedDate { get; set; }
    }
}