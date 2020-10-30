using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Samples.GameMatch.Api
{
    public class ApiStartup
    {
        private readonly IConfiguration _configuration;

        public ApiStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(o =>
                                  {
                                      o.RequireHttpsMetadata = false;
                                      o.SaveToken = true;

                                      o.TokenValidationParameters = new TokenValidationParameters
                                                                    {
                                                                        ValidateIssuer = true,
                                                                        ValidateAudience = true,
                                                                        ValidateLifetime = true,
                                                                        ValidateIssuerSigningKey = true,
                                                                        ValidIssuer = _configuration["Jwt:Issuer"],
                                                                        ValidAudience = _configuration["Jwt:Audience"],
                                                                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
                                                                        ClockSkew = TimeSpan.Zero
                                                                    };
                                  });

            services.AddMvc(o => { o.Filters.Add(new ModelAttributeValidationFilter()); })
                    .AddJsonOptions(x =>
                                    {
                                        x.JsonSerializerOptions.IgnoreNullValues = true;
                                        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                                        x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                        x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                                        x.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                                        x.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                                        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true));
                                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddSwaggerGen(c =>
                                   {
                                       c.SwaggerDoc("gamematch", new OpenApiInfo
                                                                 {
                                                                     Version = "v1",
                                                                     Title = "GameMatch API",
                                                                     Description = "GameMatch service related APIs"
                                                                 });

                                       c.AddSecurityDefinition("jwt", new OpenApiSecurityScheme
                                                                         {
                                                                             Type = SecuritySchemeType.Http,
                                                                             Description = "GameMatch JWT Token",
                                                                             In = ParameterLocation.Header,
                                                                             Name = "Bearer",
                                                                             BearerFormat = "JWT",
                                                                             Scheme = "bearer"
                                                                         });

                                       c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                                {
                                                                    {
                                                                        new OpenApiSecurityScheme
                                                                        {
                                                                            Reference = new OpenApiReference
                                                                                        {
                                                                                            Id = "jwt",
                                                                                            Type = ReferenceType.SecurityScheme
                                                                                        }
                                                                        },
                                                                        new List<string>()
                                                                    }
                                                                });

                                       foreach (var xmlFile in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
                                       {
                                           c.IncludeXmlComments(xmlFile);
                                       }
                                   });

            // Services
            services.AddSingleton<IDemoDataService, DemoDataService>()
                    .AddSingleton<ITransformer<SignupUserRequest, User>, SignupUserTransformer>()
                    .AddSingleton<ITransformer<UserRatingRequest, UserRating>, UserRatingTransformer>()
                    .AddSingleton<ITransformer<MakeMatch, Match>, MakeMatchMatchTransformer>()
                    .AddSingleton<ITransformer<UserRating, MatchPair>, MatchPairTransformer>()
                    .AddSingleton<ITransformer<MatchRequest, MakeMatch>, MatchRequestTransformer>();

            // Repos
            services.AddSingleton<IUserRepository, InMemoryUserRepository>()
                    .AddSingleton<ISettingsRepository, InMemorySettingsRepository>()
                    .AddSingleton<IUserRatingRepository, InMemoryUserRatingRepository>()
                    .AddSingleton<IMatchRepository, InMemoryMatchRepository>()
                    .AddSingleton<IMatchPairRepository, InMemoryMatchPairRepository>()
                    .AddSingleton<IObservedMatchQueue, InMemoryMatchQueue>()
                    .AddSingleton<IMatchQueue>(s => s.GetRequiredService<IObservedMatchQueue>())
                    .AddSingleton<IMatchMakerService, OneToOneMatchMakerService>();

            services.AddHostedService(s => new LocalMatchMakerProcessor(s.GetRequiredService<IObservedMatchQueue>(),
                                                                        s.GetRequiredService<IMatchMakerService>(),
                                                                        s.GetRequiredService<ILogger<LocalMatchMakerProcessor>>()));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); })
               .UseSwagger()
               .UseSwaggerUI(o =>
                             {
                                 o.SwaggerEndpoint("/swagger/gamematch/swagger.json", "GameMatch API");
                                 o.RoutePrefix = string.Empty;
                             });
        }
    }
}
