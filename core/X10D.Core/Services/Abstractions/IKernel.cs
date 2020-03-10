using System.Collections.Generic;
using IKernelPrototype = X10D.Infrastructure.IKernelFacade;
using IServicePrototype = X10D.Infrastructure.IServicePrototype;

namespace X10D.Core.Services
{
    internal interface IKernel : IKernelPrototype
    {
        /// <summary>
        /// Вывести системный токен доступа к ядру в консоль.
        /// </summary>
        void LogToken();

        // <summary>
        /// Сменить системный токен доступа к ядру в консоль.
        /// </summary>
        void ChangeToken();

        List<IServicePrototype> Services { get; }
    }
}