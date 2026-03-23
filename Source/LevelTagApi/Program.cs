using DataClasses;
using DataStoring;
using LevelTagApi.DtoConverter;
using LevelTagApi.Dtos;

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

            builder.Services.AddTransient<IDtoConverter<Level, GetLevelDto>, LevelDtoConverter>();

            var app = builder.Build();

            //using (IServiceScope scope = app.Services.CreateScope())
            //{
            //    LevelTagDbContext levelTagDb = scope.ServiceProvider.GetRequiredService<LevelTagDbContext>();
            //    levelTagDb.Database.EnsureDeleted();
            //    levelTagDb.Database.EnsureCreated();
            //}

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
