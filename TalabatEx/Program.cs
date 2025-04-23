using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Globalization;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Data.Identity;
using Talabat.Service.AuthService;
using TalabatEx.Errors;
using TalabatEx.Extensions;
using TalabatEx.Helpers;
using TalabatEx.Middleware;

namespace TalabatEx
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Dependancy Injection
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerServices();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), bu =>
                    bu.EnableRetryOnFailure());
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"), bu =>
                    bu.EnableRetryOnFailure());
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            }
            );

            //-----------------CORS------------------------
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("P1", options =>
                {
                    options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            //-----------------------------------------

            builder.Services.AddApplicationServices();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                //options.Password.RequiredUniqueChars = 2;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireDigit = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AuthKey"] ?? string.Empty))
                };
            });

            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));

            #region Swagger Auth Docs 
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWTToken_Auth_API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });
            #endregion

            #region Localization(Multi Language)

            //builder.Services.AddControllersWithViews();
            //builder.Services.AddLocalization(opt =>
            //{
            //    opt.ResourcesPath = "";
            //});

            //builder.Services.Configure<RequestLocalizationOptions>(options =>
            //{
            //    List<CultureInfo> supportedCultures = new List<CultureInfo>
            //        {
            //            new CultureInfo("en-US"),
            //            new CultureInfo("ar-EG"),
            //            new CultureInfo("fr-FR"),
            //            new CultureInfo("de-DE"),
            //        };
            //    options.DefaultRequestCulture = new RequestCulture("en-US");
            //    options.SupportedCultures = supportedCultures;
            //});

            #endregion


            #endregion


            var app = builder.Build();

            #region Update-Database With => Run

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();

            //-----------------------------------------

            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            var _userManager = services.GetRequiredService<UserManager<AppUser>>();

            //-----------------------------------------

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();


            try
            {
                await _dbContext.Database.MigrateAsync(); // Update-DataBase => StoreContext

                await StoreContextSeed.SeedAsynk(_dbContext);

                //-----------------------------------------

                await _IdentityDbContext.Database.MigrateAsync(); // Update-DataBase => AppIdentityDbContext

                await AppIdentityDbContextSeed.SeedUsersAsync(_userManager); // Data Seeding
            }
            catch (Exception e)
            {

                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e, "an error occurred during apply migration.");
            }


            #endregion

            // Configure the HTTP request pipeline.

            #region Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleware();
            }

            #region Localizaion Middleware

            //var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            //app.UseRequestLocalization(options!.Value);

            #endregion

            app.UseCors("p1");

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
