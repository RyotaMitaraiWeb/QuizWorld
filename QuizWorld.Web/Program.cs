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
using QuizWorld.Infrastructure.AuthConfig;
using QuizWorld.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using Asp.Versioning;
using QuizWorld.Web.Services.Legacy;
using QuizWorld.Web.Middlewares;
using QuizWorld.Web.Filters;
using QuizWorld.Infrastructure.AuthConfig.Handlers;
using QuizWorld.Infrastructure.AuthConfig.Requirements;
using Microsoft.AspNetCore.Authorization.Policy;
using QuizWorld.Common.Policy;
using QuizWorld.Web.Contracts.Legacy;
using QuizWorld.Infrastructure.AuthConfig.Legacy.CanAccessLogs;
using QuizWorld.Infrastructure.AuthConfig.Legacy.CanPerformOwnerAction;
using QuizWorld.Infrastructure.AuthConfig.Legacy.CanWorkWithRoles;
using QuizWorld.Infrastructure.Legacy.Filters.GuestsOnly;
using QuizWorld.Web.Hubs;
using QuizWorld.Common.Hubs;
using Microsoft.Extensions.FileProviders;

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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSingleton<IJwtServiceDeprecated, JwtServiceDeprecated>();
            builder.Services.AddSingleton<IJwtBlacklistDeprecated, JwtBlacklistServiceDeprecated>();
            builder.Services.AddSingleton<IJwtStore, JwtStore>();
            builder.Services.AddSingleton<IJwtService, JwtService>();
            builder.Services.AddScoped<AppJwtBearerEvents>();
            builder.Services.AddSingleton<GuestsOnlyFilter>();
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddScoped<IQuizServiceDeprecated, QuizServiceDeprecated>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IGradeServiceDeprecated, GradeServiceDeprecated>();
            builder.Services.AddScoped<IActivityLogger, ActivityLogger>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IRoleServiceDeprecated, RoleServiceDeprecated>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IGradeService, GradeService>();
            builder.Services.AddScoped<IAuthorizationHandler, CanWorkWithRolesHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, CanPerformOwnerActionHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, CanAccessLogsHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, CanEditAndDeleteQuizzesHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, HasRequiredRolesHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, JwtMatchesOwnUsernameHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, CreatedTheQuizHandler>();
            builder.Services.AddScoped<LogEditOrDeleteActivityFilter>();
            builder.Services.AddScoped<LogRoleChangeFilter>();
            builder.Services.AddSingleton<AuthorizationMiddlewareResultHandler>();
            builder.Services.AddSingleton<IImageService, LocalImageService>();
            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler>(sp =>
            {
                var defaultHandler = sp.GetRequiredService<AuthorizationMiddlewareResultHandler>();
                return new To404NotFoundMiddlewareResultHandler(defaultHandler);
            });

            string contentRootPath = builder.Environment.ContentRootPath;

            builder.Services.AddSingleton<IContentRootPathProvider>(new ContentRootPathProvider(contentRootPath));

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

            builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
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
                options.DescribeAllParametersInCamelCase();
                options.ResolveConflictingActions(api => api.First());
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz World v1", Version = "v1" });
                options.SwaggerDoc("v2", new OpenApiInfo { Title = "Quiz World v2", Version = "v2" });
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

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("v"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("CanEditQuiz", policy =>
                {
                    policy.Requirements.Add(new CanPerformOwnerActionRequirement(Roles.Moderator));
                })
                .AddPolicy("CanDeleteQuiz", policy =>
                {
                    policy.Requirements.Add(new CanPerformOwnerActionRequirement(Roles.Moderator));
                })
                .AddPolicy("CanAccessLogs", policy =>
                {
                    policy.Requirements.Add(new CanAccessLogsRequirement(Roles.Admin));
                })
                .AddPolicy("CanSeeRoles", policy =>
                {
                    policy.Requirements.Add(new CanWorkWithRolesRequirement(false, Roles.Admin));
                })
                .AddPolicy("CanChangeRoles", policy =>
                {
                    policy.Requirements.Add(new CanWorkWithRolesRequirement(true, Roles.Admin));
                })
                .AddPolicy(CanEditAndDeleteQuizzesHandler.Name, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new CanEditAndDeleteQuizzesRequirement(Roles.Moderator));
                })
                .AddPolicy(PolicyNames.CanViewLogs, policy =>
                {
                    policy.Requirements.Add(new HasRolesRequirement(Roles.Admin));
                })
                .AddPolicy(PolicyNames.CanInteractWithRoles, policy =>
                {
                    policy.Requirements.Add(new HasRolesRequirement(Roles.Admin));
                })
                .AddPolicy(JwtMatchesOwnUsernameHandler.Name, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new JwtMatchesOwnUsernameRequirement());
                });

            
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        string origins = builder.Configuration["ALLOWED_HOSTS"] ?? throw new Exception("No origins specified in environment variables");
                        
                        policy.WithOrigins(origins.Split(", "));
                        policy.AllowAnyHeader();
                        policy.WithMethods("GET", "PUT", "POST", "DELETE", "PATCH");
                        policy.AllowCredentials();
                    });
            });

            var app = builder.Build();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            Console.WriteLine(Path.Combine(contentRootPath, LocalImageService.BasePath));

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(contentRootPath, LocalImageService.BasePath)
                    ),
                    RequestPath = "/images"
                }
            );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthentication();
            app.UseAttachQuizToContext();
            app.UseAuthorization();
            app.MapHub<SessionHub>(HubEndpoints.Session);

            app.SeedAdministrator("admin", builder.Configuration["ADMIN_PASS"]);

            app.MapControllers();
            

            app.Run();
        }
    }
}