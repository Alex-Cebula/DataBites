using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIS341_project_cebula.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CIS341_project_cebula
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()){
                var services = scope.ServiceProvider;

                try
                {
                    //Get contexts
                    var dataBitesContext = services.GetRequiredService<DataBitesContext>();
                    var userContext = services.GetRequiredService<UserContext>();
                    //Initialize application database
                    DataBitesInitializer.Initialize(dataBitesContext, userContext);
                    //Initialize .net identity database
                    UserInitializer.Initialize(services).Wait();
                }
                catch(Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Error seeding database");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
