using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace X10D.Mvc.Formats
{
    /// <summary>
    /// Общий формат API ответа.
    /// </summary>
    public interface IApiResponse
    {
#pragma warning disable IDE1006 // Стили именования        
        /// <summary>
        /// Если поле <see cref="ok"/> равно <c>true</c> 
        /// (<see cref="status_code"/> == <see cref="StatusCodes.Status200OK"/>), 
        /// запрос был выполнен успешно, в таком случае 
        /// результат будет находиться в поле <see cref="result"/>.
        /// 
        /// В случае неудачного запроса <see cref="ok"/> равно <c>false</c> 
        /// (<see cref="status_code"/> != <see cref="StatusCodes.Status200OK"/>), 
        /// а ошибка приведена в поле <see cref="description"/>.
        /// </summary>
        bool ok { get; }

        /// <summary>
        /// Результат выполнения запроса.
        /// </summary>
        object result { get; }

        /// <summary>
        /// Код ответа сервера.
        /// </summary>
        int status_code { get; }

        /// <summary>
        /// Дополнительная информация по коду ответа от сервера, 
        /// как правило, описание ошибки.
        /// </summary>
        string description { get; }

        /// <summary>
        /// Время обработки запроса сервером в мс.
        /// </summary>
        double? request_time { get; }
#pragma warning restore IDE1006 // Стили именования

        StringBuilder Pack();
    }
}
