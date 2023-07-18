using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizWorld.Infrastructure.Data;
using QuizWorld.Infrastructure.Data.Entities;
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
using Microsoft.AspNetCore.Authorization.Policy;
using QuizWorld.Web.Services.GradeService;

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
            builder.Services.AddHostedService<IndexCreationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["REDIS_CONNECTION_STRING"]));
            builder.Services.AddSingleton<IJwtService, JwtService>();
            builder.Services.AddSingleton<IJwtBlacklist, JwtBlacklistService>();
            builder.Services.AddScoped<AppJwtBearerEvents>();
            builder.Services.AddSingleton<GuestsOnlyFilter>();
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IGradeService, GradeService>();
            builder.Services.AddScoped<IAuthorizationHandler, CanPerformOwnerActionHandler>();

            builder.Services.AddDbContext<QuizWorldDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
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
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
                   
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
                    policy.Requirements.Add(new CanPerformOwnerActionRequirement("Moderator"));
                });
                options.AddPolicy("CanDeleteQuiz", policy =>
                {
                    policy.Requirements.Add(new CanPerformOwnerActionRequirement("Moderator"));
                });
            });

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}