using Distags.Api.Contracts;
using Distags.Api.Hubs;
using Distags.Api.Security;
using Distags.Core;
using Distags.Core.Events;
using Distags.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Serilog;
using SerilogUtils;
using SteamWebApi;
using System.Net.Http.Headers;

namespace Distags.Api;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureConsoleAndFileSinks($"{nameof(Distags)}.{nameof(Api)}")
            .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(builder.Configuration)
                .ConfigureConsoleAndFileSinks($"{nameof(Distags)}.{nameof(Api)}"));

            AddAuthentication(builder);

            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new PatchValueConverterFactory()));
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSignalR();

            string distagsDbConnectionString = builder.Configuration.GetConnectionString("DistagsDatabase") ??
                                               throw new InvalidOperationException("Connection string not configured.");
            builder.Services.AddDistagsPersistence(distagsDbConnectionString);
            builder.Services.AddDistagsCore();

            builder.Services.AddSteamWebApi("SteamWebApi");

            AddValidatedConfigOptions<LevelTagApiSettings>(builder, "LevelTagApiSettings");

            builder.Services.AddSingleton<ITicketStore<long>, ConcurrentTicketStore<long>>();
            builder.Services.AddTransient<ITokenGenerator, RandomBase64UrlTokenGenerator>();

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo(typeof(IAppEventHandler<>)))
                .AsImplementedInterfaces());

            builder.Services.AddHostedService<SessionCleanupService>();

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
}