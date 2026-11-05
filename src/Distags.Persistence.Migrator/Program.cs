using AppHostUtils;
using Distags.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;

namespace Distags.Persistence.Migrator;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);

            hostBuilder.Configuration.AddDockerSecrets();
            hostBuilder.Configuration.ReplaceConnectionStringPassword("DistagsDatabase");

            string connectionString = hostBuilder.Configuration.GetRequiredConnectionString("DistagsDatabase");
            hostBuilder.Services.AddDistagsPersistence(
                connectionString,
                npgsqlOptionsAction: npgsqlOptions =>
                    npgsqlOptions.MigrationsAssembly($"{nameof(Distags)}.{nameof(Persistence)}.{nameof(Migrations)}"));
            hostBuilder.Services.AddDistagsCore();

            IHost host = hostBuilder.Build();

            AsyncServiceScope scope = host.Services.CreateAsyncScope();
            await using ConfiguredAsyncDisposable awaitedScope = scope.ConfigureAwait(false);

            DistagsDbContext dbContext = scope.ServiceProvider.GetRequiredService<DistagsDbContext>();

            Console.WriteLine("Applying EF Core migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Done!");
            return 0;
        }
        catch (OperationCanceledException)
        {
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}
