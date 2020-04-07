using ExtCore.Infrastructure.Actions;
using Microsoft.Extensions.DependencyInjection;
using System;
using X10D.Mvc.Formats;

namespace X10D.Mvc.Actions
{
    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public int Priority => 1000;

        public void Execute(IServiceCollection services, IServiceProvider serviceProvider)
        {
            services
                .AddMvc(options =>
                {
                    options.OutputFormatters.Insert(0, new DefaultFormatter());
                });
        }
    }
}
