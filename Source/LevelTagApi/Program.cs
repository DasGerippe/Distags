using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Filtering;
using LevelTagApi.Hubs;
using LevelTagApi.Sorting;

namespace LevelTagApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            string levelTagsDbConnectionString = builder.Configuration.GetConnectionString("LevelTagsDatabase") ??
                                                 throw new InvalidOperationException("Connection string not configured.");
            builder.Services.AddPostgreSqlLevelTagDbContext(levelTagsDbConnectionString);

            builder.Services.AddSignalR();

            builder.Services.AddTransient<IQueryableFilterFactory<Level, GetLevelsParameters>, BrowseLevelsFilterFactory>();
            builder.Services.AddTransient<IQueryableSorterFactory<Level, LevelSortStrategy>, LevelSorterFactory>();

            var app = builder.Build();

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
    }
}
