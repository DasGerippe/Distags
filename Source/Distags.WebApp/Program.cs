using Distags.WebApp.Components;
using Distags.WebApp.DistagsApi;
using Distags.WebApp.Security;
using Distags.WebApp.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Serilog;
using SerilogUtils;

namespace Distags.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ConfigureConsoleAndFileSinks($"{nameof(Distags)}.{nameof(WebApp)}")
            .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog(loggerConfig => loggerConfig
                .ReadFrom.Configuration(builder.Configuration)
                .ConfigureConsoleAndFileSinks($"{nameof(Distags)}.{nameof(WebApp)}"));

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddHttpClient<DistagsApiClient>(httpClient =>
            {
                string distagsApiUri = builder.Configuration["DistagsWebApp:DistagsApiUri"]!;
                httpClient.BaseAddress = new Uri(distagsApiUri);
            });

            builder.Services.AddLocalization();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddTransient<HistoryManager>();
            builder.Services.AddTransient<LevelUrlProvider>();
            builder.Services.AddTransient<LevelCreatorNameProvider>();

            builder.Services.AddScoped<CircuitHandler, CircuitSessionTokenInitializer>();
            builder.Services.AddScoped<ISessionTokenProvider, SessionTokenProvider>();
            builder.Services.AddScoped<IDialogService, DialogService>();
            builder.Services.AddScoped<TagTreeProvider>();

            builder.Services.AddScoped<TimeRefreshService>();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.UseMiddleware<TicketProcessingMiddleware>();
            app.UseMiddleware<SessionTokenReadingMiddleware>();
            app.UseMiddleware<LogoutProcessingMiddleware>();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

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
}
