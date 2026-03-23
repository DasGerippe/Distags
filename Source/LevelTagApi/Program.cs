using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Filtering;
using LevelTagApi.Hubs;
using LevelTagApi.Security;
using LevelTagApi.Sorting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SteamWebApi;

namespace LevelTagApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            AddAuthentication(builder);
            AddAuthorization(builder);

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            string levelTagsDbConnectionString = builder.Configuration.GetConnectionString("LevelTagsDatabase") ??
                                                 throw new InvalidOperationException("Connection string not configured.");
            builder.Services.AddPostgreSqlLevelTagDbContext(levelTagsDbConnectionString);

            builder.Services.AddSignalR();

            builder.Services.AddSteamWebApi("SteamWebApi");

            AddValidatedConfigOptions<LevelTagApiSettings>(builder, "LevelTagApiSettings");

            builder.Services.AddTransient<IQueryableFilterFactory<Level, GetLevelsParameters>, BrowseLevelsFilterFactory>();
            builder.Services.AddTransient<IQueryableSorterFactory<Level, LevelSortStrategy>, LevelSorterFactory>();

            builder.Services.AddSingleton<ITicketStore<long>, ConcurrentTicketStore<long>>();
            builder.Services.AddTransient<ITokenGenerator, RandomBase64UrlTokenGenerator>();

            builder.Services.AddTransient<RoleProvider>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<LevelHub>("/levelHub");

            app.Run();
        }

        private static void AddValidatedConfigOptions<TOptions>(WebApplicationBuilder builder, string configSectionPath)
            where TOptions : class
        {
            builder.Services
                .AddOptions<TOptions>()
                .BindConfiguration(configSectionPath)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddTransient<TOptions>(services => services.GetRequiredService<IOptions<TOptions>>().Value);
        }

        private static void AddAuthentication(WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Constants.Session.AuthenticationScheme;
                    options.DefaultChallengeScheme = Constants.Session.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
                    Constants.Session.AuthenticationScheme,
                    options => {})
                .AddCookie(Constants.Steam.LoginCookieScheme)
                .AddSteam(options =>
                {
                    options.SignInScheme = Constants.Steam.LoginCookieScheme;
                });
        }

        private static void AddAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    Constants.Policies.Tagging,
                    policy => policy.Requirements.Add(new TaggingAuthorizationRequirement()));
            });

            builder.Services.AddScoped<IAuthorizationHandler, TaggingAuthorizationHandler>();
        }
    }
}
