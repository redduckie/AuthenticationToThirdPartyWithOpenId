using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net;
using X509Helper;

namespace Authentication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Authentication";
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                //kestrel stuff
                .UseKestrel(options =>
                {
                    //options.Listen(IPAddress.Any, 5001, listenOptions =>
                    //{
                    //    listenOptions.UseHttps(X509.GetCertificate("D73F68A4373AE4A48780667578E063BCF414D7A3"));
                    //});
                })
                .UseIISIntegration()
                .UseSerilog((context, config) =>
                {
                    config
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.File(@"identityserver4_log.txt")
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}]  {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);

                });
        }
    }
}
