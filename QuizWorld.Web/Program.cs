using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizWorld.Infrastructure.Data;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Services;
using Redis.OM.Skeleton.HostedServices;
using Redis.OM;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.Web.Services.JsonWebToken;
using QuizWorld.Infrastructure.Filters.GuestsOnly;
using QuizWorld.Infrastructure.AuthConfig;
using QuizWorld.Infrastructure.AuthConfig.CanPerformOwnerAction;
using QuizWorld.Web.Contracts.Quiz;
using QuizWorld.Web.Services.QuizService;
using QuizWorld.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using QuizWorld.Web.Services.GradeService;
using QuizWorld.Infrastructure.AuthConfig.CanAccessLogs;
using QuizWorld.Web.Contracts.Logging;
using QuizWorld.Web.Services.Logging;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Web.Contracts.Roles;
using QuizWorld.Web.Services.RoleService;
using QuizWorld.Infrastructure.AuthConfig.CanWorkWithRoles;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;

namespace QuizWorld.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var auth = builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging();

            ConfigurationOptions options = new()
            {
                EndPoints = { builder.Configuration["REDIS_CONNECTION_STRING"] },
                ConnectTimeout = 15000,
                SyncTimeout = 15000,
                AbortOnConnectFail = false,
            };

            builder.Services.AddSingleton(new RedisConnectionProvider(options));
            builder.Services.AddHostedService<IndexCreationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton<IJwtService, JwtService>();
            builder.Services.AddSingleton<IJwtBlacklist, JwtBlacklistService>();
            builder.Services.AddScoped<AppJwtBearerEvents>();
            builder.Services.AddSingleton<GuestsOnlyFilter>();
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IGradeService, GradeService>();
            builder.Services.AddScoped<IActivityLogger, ActivityLogger>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IAuthorizationHandler, CanWorkWithRolesHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, CanPerformOwnerActionHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, CanAccessLogsHandler>();

            builder.Services.AddDbContext<QuizWorldDbContext>(options =>
            {
                SqlConnectionStringBuilder connBuilder = new()
                {
                    Password = builder.Configuration["DB_PASSWORD"],
                    DataSource = builder.Configuration["DB_HOST"],
                    ConnectRetryCount = 30,
                    InitialCatalog = builder.Configuration["DB_NAME"],
                    UserID = builder.Configuration["DB_USER"],
                    Encrypt = false,
                };
                options.UseSqlServer(connBuilder.ConnectionString);
            });

            

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<QuizWorldDbContext>();

            auth.AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = builder.Configuration["JWT_VALID_AUDIENCE"],
                    ValidIssuer = builder.Configuration["JWT_VALID_ISSUER"],
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET"])),
                   
                };

                options.EventsType = typeof(AppJwtBearerEvents);
            });
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                }).AddNewtonsoftJson(options =>
                {
                    
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy
                        {
                            ProcessDictionaryKeys = true,
                            ProcessExtensionDataNames= true,
                        }
                    };
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.ResolveConflictingActions(api => api.First());
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz World", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanEditQuiz", policy =>
                {
                    policy.Requirements.Add(new CanPerformOwnerActionRequirement(Roles.Moderator));
                });

                options.AddPolicy("CanDeleteQuiz", policy =>
                {
                    policy.Requirements.Add(new CanPerformOwnerActionRequirement(Roles.Moderator));
                });

                options.AddPolicy("CanAccessLogs", policy =>
                {
                    policy.Requirements.Add(new CanAccessLogsRequirement(Roles.Admin));
                });

                options.AddPolicy("CanSeeRoles", policy =>
                {
                    policy.Requirements.Add(new CanWorkWithRolesRequirement(false, Roles.Admin));
                });

                options.AddPolicy("CanChangeRoles", policy =>
                {
                    policy.Requirements.Add(new CanWorkWithRolesRequirement(true, Roles.Admin));
                });
            });


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        string origins = builder.Configuration["ALLOWED_HOSTS"] ?? throw new Exception("No origins specified in environment variables");
                        
                        policy.WithOrigins(origins.Split(", "));
                        policy.AllowAnyHeader();
                        policy.WithMethods("GET", "PUT", "POST", "DELETE");
                    });
            });

            var app = builder.Build();
            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();



            app.SeedAdministrator("admin", builder.Configuration["ADMIN_PASS"]);

            app.MapControllers();

            app.Run();
        }
    }
}