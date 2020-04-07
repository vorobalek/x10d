using Microsoft.Extensions.DependencyInjection;
using System;

namespace X10D.Infrastructure.Attributes
{
    public sealed class ServiceAttribute : Attribute
    {
        public Type InterfaceType { get; set; }
        public Func<IServiceProvider, object> CustomFactory { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; }
    }
}
