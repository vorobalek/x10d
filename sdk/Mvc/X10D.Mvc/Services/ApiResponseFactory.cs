using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X10D.Infrastructure;
using X10D.Mvc.Attributes;
using X10D.Mvc.Formats;

namespace X10D.Mvc.Services
{
    internal sealed class ApiResponseFactory : ServicePrototype<IApiResponseFactory>, IApiResponseFactory
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;

        private IActivator Activator { get; }

        public IReadOnlyList<string> Formats => formats.Keys.ToList().AsReadOnly();

        public ApiResponseFactory(IActivator activator)
        {
            Activator = activator;
        }

        protected override void PrepareService()
        {
            formats = new Dictionary<string, Type>(
                Activator
                .GetImplementations<IApiResponse>()
                .Where(t => !t.IsAbstract && t.GetCustomAttribute<FormatAttribute>() is FormatAttribute f)
                .Select(t => new KeyValuePair<string, Type>(t.GetCustomAttribute<FormatAttribute>().Name, t)));

            base.PrepareService();
        }

        protected override void FlushService()
        {
            formats = null;
            base.FlushService();
        }

        public Type GetFormatType(string format)
        {
            return formats.GetValueOrDefault(format);
        }

        private Dictionary<string, Type> formats;
    }
}
