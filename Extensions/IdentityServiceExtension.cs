using System.Text;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using VAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;

namespace VAPI.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddScoped<TokenService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                        
            services.AddIdentityCore<AppUser>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddDefaultTokenProviders();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => 
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("DeleteMessagePolicy", policy =>
                {
                    policy.Requirements.Add(new DeleteMessagePolicy());
                }); 

                opt.AddPolicy("DeleteCommentPolicy", policy =>
                {
                    policy.Requirements.Add(new DeleteCommentPolicy());
                }); 

                opt.AddPolicy("CommandPolicy", policy =>
                {
                    policy.Requirements.Add(new CommandPolicy());
                });
            });

            services.AddTransient<IAuthorizationHandler, CommandPolicyHandler>();
            services.AddTransient<IAuthorizationHandler, DeleteMessagePolicyHandler>();
            services.AddTransient<IAuthorizationHandler, DeleteCommentPolicyHandler>();



            return services;
        }
        
    }
}