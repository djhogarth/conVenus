using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();

            //create a scope for the services
            var services = scope.ServiceProvider;
            try
            {
              /* we want to get our DataContext service so we can pass it
              into our Seed method */
              var context = services.GetRequiredService<DataContext>();
              var userManager = services.GetRequiredService<UserManager<AppUser>>();
              await context.Database.MigrateAsync();
              await Seed.SeedUsers(userManager);
            }
            catch(Exception ex)
            {
              var logger = services.GetRequiredService<Logger<Program>>();
              logger.LogError(ex, "An error has occured during migration");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
