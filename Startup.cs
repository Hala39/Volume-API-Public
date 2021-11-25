using API.Extensions;
using VAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using VAPI.SignalR;
using Microsoft.Extensions.Hosting;

namespace VAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VAPI", Version = "v1" });
            }); 
        
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .WithExposedHeaders("Pagination", "WWW-Authenticate")
                           .WithOrigins("https://localhost:4200");
                });
            });
            services.AddApplicationServices(Configuration);
            services.AddIdentityServices(Configuration);
            services.AddDbServices(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(options => options.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());
            app.UseCsp(options => options   
                .BlockAllMixedContent()
                .StyleSources(s => s
                    .Self()
                    .UnsafeInline()
                )
                .FontSources(s => s.Self()
                    .CustomSources("https://fonts.gstatic.com", "data:"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self()
                    .CustomSources(
                        "https://res.cloudinary.com", 
                        "https://unpkg.com", 
                        "https://graph.facebook.com/",
                        "https://www.facebook.com/",
                        "https://platform-lookaside.fbsbx.com/",
                        "https://lh3.googleusercontent.com/"
                    )
                )
                .ScriptSources(s => s
                    .Self()
                    .UnsafeInline() 
                    .CustomSources(
                        "https://connect.facebook.net/en_US/sdk.js",
                        "https://apis.google.com/"
                    )
                )
            );

            if (env.IsProduction())
            {
                app.Use(async (context, next) => {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
                    await next.Invoke();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
                endpoints.MapHub<CommentHub>("hubs/comment");
                endpoints.MapHub<LikeHub>("hubs/like");
                endpoints.MapHub<FollowHub>("hubs/follow");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
