using DataClasses;
using DataStoring;
using LevelTagApi.Dtos;
using LevelTagApi.Filtering;
using LevelTagApi.Hubs;
using LevelTagApi.Security;
using LevelTagApi.Sorting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            AddValidatedConfigOptions<LevelTagApiSettings>(builder, "LevelTagApiSettings");
            AddValidatedConfigOptions<AccessTokenSettings>(builder, "LevelTagApiSettings:AccessTokenSettings");

            builder.Services.AddTransient<IQueryableFilterFactory<Level, GetLevelsParameters>, BrowseLevelsFilterFactory>();
            builder.Services.AddTransient<IQueryableSorterFactory<Level, LevelSortStrategy>, LevelSorterFactory>();

            builder.Services.AddSingleton<ITicketStore<long>, ConcurrentTicketStore<long>>();
            builder.Services.AddTransient<ITicketGenerator, GuidTicketGenerator>();
            builder.Services.AddTransient<IRefreshTokenGenerator, RandomRefreshTokenGenerator>();
            builder.Services.AddTransient<IAccessTokenGenerator, JwtAccessTokenGenerator>();

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
            string accessTokenIssuer = builder.Configuration["LevelTagApiSettings:AccessTokenSettings:Issuer"] ??
                                       throw new InvalidOperationException("JWT token issuer not configured.");
            string accessTokenAudience = builder.Configuration["LevelTagApiSettings:AccessTokenSettings:Audience"] ??
                                         throw new InvalidOperationException("JWT token audience not configured.");
            string accessTokenSecretKey = builder.Configuration["LevelTagApiSettings:AccessTokenSettings:SecretKey"] ??
                                          throw new InvalidOperationException("JWT token signing key not configured.");

            builder.Services
                .AddAuthentication(options =>
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
                        ValidIssuer = accessTokenIssuer,
                        ValidAudience = accessTokenAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecretKey)),
                    };
                })
                .AddCookie("SteamLoginCookie")
                .AddSteam(options =>
                {
                    options.SignInScheme = "SteamLoginCookie";
                });
        }

        private static void AddAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Auth.PostAutoTagPolicy, policy => policy.Requirements.Add(new PostAutoTagRequirement()));
            });

            builder.Services.AddScoped<IAuthorizationHandler, PostAutoTagHandler>();
        }
    }
}
