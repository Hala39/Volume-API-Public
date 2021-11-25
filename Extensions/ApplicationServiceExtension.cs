using VAPI;
using VAPI.Helpers;
using VAPI.Interfaces;
using VAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VAPI.SignalR;
using MediatR;
using VAPI.MediatR.Posts;
using VAPI.Data;
using API.Helpers;
using VAPI.Email;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddSingleton<PresenceTracker>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IGroupRepository,GroupRepository>();

            services.AddScoped<LogUserActivity>();
            services.AddScoped<EmailSender>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddAutoMapper(typeof(Startup));

            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddScoped<Mediator>();
            services.AddSignalR();

            return services;
        }
    }
}