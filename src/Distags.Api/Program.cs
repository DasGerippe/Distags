using AppHostUtils;
using AutoTaggerWorker;
using Distags.Api.Caching;
using Distags.Api.Contracts;
using Distags.Api.Hubs;
using Distags.Api.Security;
using Distags.Core;
using Distags.Core.Events;
using Distags.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
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
            .ConfigureBootstrapLogger()
            .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddDockerSecrets();
            builder.Configuration.ReplaceConnectionStringPassword("DistagsDatabase");

            builder.Logging.ClearProviders();
            builder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(builder.Configuration)
                .ConfigureSinks<Program>(writeToFile: builder.Environment.IsDevelopment(), OutputTemplates.Web));

            AddAuthentication(builder);
            AddOutputCaching(builder);
            AddProblemDetails(builder);

            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new PatchValueConverterFactory()));
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSignalR();

            builder.Services.AddDistagsPersistence(builder.Configuration.GetRequiredConnectionString("DistagsDatabase"));
            builder.Services.AddDistagsCore();

            builder.Services.AddSteamWebApi("SteamWebApi");

            builder.Services.AddValidatedConfigOptions<DistagsApiSettings>("Distags:Api");

            builder.Services.AddSingleton<ITicketStore<long>, ConcurrentTicketStore<long>>();
            builder.Services.AddTransient<ITokenGenerator, RandomBase64UrlTokenGenerator>();

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo(typeof(IAppEventHandler<>)))
                .AsImplementedInterfaces());

            builder.Services.AddHostedService<SessionCleanupService>();
            builder.Services.AddAutoTaggerWorker();

            builder.Services.AddHealthChecks();

            WebApplication app = builder.Build();

            app.UseExceptionHandler();
            app.UseStatusCodePages();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseOutputCache();

            app.MapStaticAssets();
            app.MapControllers();

            app.MapHealthChecks("/health");

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
                        DistagsApiConstants.AuthorizationHeaderSchemes.SessionToken => Constants.Authentication.SessionTokenScheme.Name,
                        _ => anonymousAuthScheme,
                    };
                };
            })
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(
                Constants.Authentication.SessionTokenScheme.Name,
                Constants.Authentication.SessionTokenScheme.DisplayName,
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

    private static void AddOutputCaching(WebApplicationBuilder builder)
    {
        builder.Services.AddOutputCache(options => options
            .AddPolicy(
                Constants.Cache.IgnoreAuthPolicyName,
                policyBuilder => policyBuilder
                    .AddPolicy<IgnoreAuthCachePolicy>(),
                excludeDefaultPolicy: true));
    }

    private static void AddProblemDetails(WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.Configure<ProblemDetailsOptions>(options => options.CustomizeProblemDetails += context =>
            {
                Exception? exception = context.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
                if (exception == null)
                    return;

                context.ProblemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
                context.ProblemDetails.Extensions["exceptionMessage"] = exception.Message;
                context.ProblemDetails.Extensions["exceptionStacktrace"] = exception.StackTrace;
            });
        }
    }
}
