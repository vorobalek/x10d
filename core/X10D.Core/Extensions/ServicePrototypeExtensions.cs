using Microsoft.Extensions.Logging;
using System;
using X10D.Infrastructure;

namespace X10D.Core.Extensions
{
    internal static class ServicePrototypeExtensions
    {
        internal static void LogServiceChangeState(this IServicePrototype service, ILogger logger, ServiceState state)
        {
            logger.LogWarning($"{service.GetType().GetFullName()} {state}");
        }
    }
}
