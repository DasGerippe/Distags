using DataClasses;
using DataStoring;
using LevelTagApi.Conversion;
using LevelTagApi.Dtos;
using LevelTagApi.Filtering;
using LevelTagApi.Hubs;
using LevelTagApi.Interceptors;
using LevelTagApi.Management;
using LevelTagApi.Security;
using LevelTagApi.Sorting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Serilog;
using SerilogUtils;
using SteamWebApi;
using System.Net.Http.Headers;

namespace LevelTagApi;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureConsoleAndFileSinks(nameof(LevelTagApi))
            .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(builder.Configuration)
                .ConfigureConsoleAndFileSinks(nameof(LevelTagApi)));

            AddAuthentication(builder);
            AddAuthorization(builder);

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            string levelTagsDbConnectionString = builder.Configuration.GetConnectionString("LevelTagsDatabase") ??
                                                 throw new InvalidOperationException("Connection string not configured.");
            builder.Services.AddNpgsqlLevelTagDbContext(levelTagsDbConnectionString, (sp, options) =>
            {
                options.AddInterceptors(sp.GetRequiredService<ContributionTrackingInterceptor>());
                options.AddInterceptors(sp.GetRequiredService<LevelTagBroadcastingInterceptor>());
                options.AddInterceptors(sp.GetRequiredService<ContributionBroadcastingInterceptor>());
            });
            builder.Services.AddSingleton<ContributionTrackingInterceptor>();
            builder.Services.AddSingleton<LevelTagBroadcastingInterceptor>();
            builder.Services.AddSingleton<ContributionBroadcastingInterceptor>();

            builder.Services.AddSignalR();
            builder.Services.AddTransient<ILevelTagBroadcaster, LevelTagBroadcaster>();
            builder.Services.AddTransient<IContributionBroadcaster, ContributionBroadcaster>();

            builder.Services.AddSteamWebApi("SteamWebApi");

            AddValidatedConfigOptions<LevelTagApiSettings>(builder, "LevelTagApiSettings");

            builder.Services.AddTransient<ContributionDtoConverter>();

            builder.Services.AddTransient<IQueryableFilterFactory<Level, BrowseLevelsParameters>, BrowseLevelsFilterFactory>();
            builder.Services.AddTransient<IQueryableSorterFactory<Level, LevelSortStrategy>, LevelSorterFactory>();

            builder.Services.AddTransient<IAppManager, AppManager>();
            builder.Services.AddTransient<IUserManager, UserManager>();
            builder.Services.AddTransient<ILevelTagManager, LevelTagManager>();
            builder.Services.AddTransient<IContributionManager, ContributionManager>();

            builder.Services.AddSingleton<ITicketStore<long>, ConcurrentTicketStore<long>>();
            builder.Services.AddTransient<ITokenGenerator, RandomBase64UrlTokenGenerator>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<LevelHub>("/hubs/levels");
            app.MapHub<ContributionHub>("/hubs/contributions");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error occurred causing the Host to terminate.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
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
        const string autoAuthScheme = "AutoAuth";
        const string anonymousAuthScheme = "Anonymous";
        const string steamLoginCookieScheme = "SteamLoginCookie";

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = autoAuthScheme;
                options.DefaultChallengeScheme = autoAuthScheme;
            })
            .AddPolicyScheme(autoAuthScheme, null, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    AuthenticationHeaderValue.TryParse(
                        context.Request.Headers.Authorization.ToString(), out AuthenticationHeaderValue? authHeader);

                    return authHeader?.Scheme switch
                    {
                        LevelTagApiConstants.AuthorizationHeaderSchemes.SessionToken => Constants.Authentication.SessionTokenScheme.Name,
                        LevelTagApiConstants.AuthorizationHeaderSchemes.ApiKey => Constants.Authentication.ApiKeyScheme.Name,
                        _ => anonymousAuthScheme,
                    };
                };
            })
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
                Constants.Authentication.SessionTokenScheme.Name,
                Constants.Authentication.SessionTokenScheme.DisplayName,
                _ => { })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                Constants.Authentication.ApiKeyScheme.Name,
                Constants.Authentication.ApiKeyScheme.DisplayName,
                _ => { })
            .AddScheme<AuthenticationSchemeOptions, AnonymousAuthenticationHandler>(
                anonymousAuthScheme,
                _ => { })
            .AddCookie(steamLoginCookieScheme)
            .AddSteam(options =>
            {
                options.SignInScheme = steamLoginCookieScheme;
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