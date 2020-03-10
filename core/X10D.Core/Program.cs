using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace X10D.Core
{
    internal sealed class Program
    {
        internal static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel()
                    .UseStartup<Startup>();
                })
                .UseDefaultServiceProvider((ctx, opts) =>
                {
                    opts.ValidateScopes = false;
                    opts.GetType()
                        .GetProperty("Mode", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                        .SetValue(opts, 1);
                });
    }
}
