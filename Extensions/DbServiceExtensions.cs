using System;
using VAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VAPI.Extensions
{
    public static class DbServiceExtensions
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
                string connStr;
            
                if (env == "Development")
                {
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];
            
                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SslMode=Require;trustServerCertificate=true";
                }

                options.UseNpgsql(connStr);
            });
            
            return services;

        }
    }
}