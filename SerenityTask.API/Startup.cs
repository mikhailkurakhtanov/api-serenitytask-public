using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Services.Implementations;
using SerenityTask.API.Models.Client.Authentication;
using SerenityTask.API.Hubs;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<ISystemMaintenanceService, SystemMaintenanceService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<ITaskService, TaskService>()
                .AddScoped<IGoogleIntegrationService, GoogleIntegrationService>()
                .AddScoped<ITaskHistoryNoteService, TaskHistoryNoteService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IPlantService, PlantService>()
                .AddScoped<IQuoteService, QuoteService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IUserDetailsService, UserDetailsService>()
                .AddScoped<ISessionService, SessionService>()
                .AddScoped<IUserNotificationService, UserNotificationService>()
                .AddScoped<IHubService, HubService>()
                .AddScoped<ITimerHubService, TimerHubService>();

            services.AddDbContext<SerenityTaskDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("Default")));

            var authenticationOptionsConfiguration = Configuration.GetSection("JwtConfiguration");
            services.Configure<AuthenticationOptions>(authenticationOptionsConfiguration);

            var authenticationOptions = authenticationOptionsConfiguration.Get<AuthenticationOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = authenticationOptions.Issuer,
                        ValidAudience = authenticationOptions.Audience,
                        IssuerSigningKey = authenticationOptions.GetSymmetricSecurityKey()
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken)) context.Token = accessToken;

                            // if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/plant") ||  path.StartsWithSegments("/auth"))
                            // {
                            //     context.Token = accessToken;
                            // }
                            return Task.CompletedTask;
                        }
                    };
                });

            var tokenAudience = authenticationOptions.Audience;
            var nonProtocolUrl = tokenAudience
                .Substring(tokenAudience.IndexOf("//"), tokenAudience.Length - tokenAudience.IndexOf("//"));
            var httpUrl = "http:" + nonProtocolUrl;
            var httpsUrl = "https:" + nonProtocolUrl;

            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(httpUrl, httpsUrl)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetIsOriginAllowed(IsOriginAllowed);
            }));

            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PlantHub>("/plant");
                endpoints.MapHub<TaskHub>("/task");
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapHub<SessionHub>("/session");
                endpoints.MapHub<TimerHub>("/timer");
                endpoints.MapHub<AuthorizationHub>("/auth");
                endpoints.MapHub<UserDetailsHub>("/user-details");
                endpoints.MapHub<UserNotificationHub>("/user-notification");
                endpoints.MapControllers();
                // endpoints.MapGet("/user-details", context => context.Response.WriteAsync("user-details"));
            });
        }

        private static bool IsOriginAllowed(string host)
        {
            return true;
        }
    }
}
