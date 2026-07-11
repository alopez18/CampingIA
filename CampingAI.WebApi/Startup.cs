using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace CampingAI.WebApi;
public class Startup {
    public static WebApplication Init(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        WebApplication app = builder.Build();
        ConfigureApp(app);
        return app;
    }



    private static void ConfigureServices(WebApplicationBuilder builder) {
        Config.DI_Manager.Configure(builder.Services, builder.Configuration);
        builder.Services.AddControllersWithViews();
        builder.Services.AddControllers()
        .AddJsonOptions(options => {
            options.JsonSerializerOptions.Converters.Add(new Converters.DateTimeConverter());
        });


        var clientIdSection = builder.Configuration.GetRequiredSection("GoogleAuth:ClientId");
        var clientSecretSection = builder.Configuration.GetRequiredSection("GoogleAuth:ClientSecret");
        if (string.IsNullOrWhiteSpace(clientIdSection.Value) || string.IsNullOrWhiteSpace(clientSecretSection.Value)) {
            throw new Exception("The client Id and the client secret of google auth value are required");
        }


        builder.Services.AddAuthentication(options => {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options => {

                options.ClientId = clientIdSection.Value;
                options.ClientSecret = clientSecretSection.Value;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.CallbackPath = "/signin-google";

                options.Events.OnCreatingTicket = context => {
                    var accessToken = context.AccessToken;
                    if (accessToken != null && context.Identity != null)
                        context.Identity.AddClaim(new System.Security.Claims.Claim("access_token", accessToken));
                    return Task.CompletedTask; // Devolver una tarea completada
                };
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                var jwtSettings = builder.Configuration.GetSection(Settings.JwtSettings.SECTION);
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is required"));
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });


        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null) {
            throw new InvalidOperationException("Entry assembly not found.");
        }
        var attribute = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute == null) {
            throw new InvalidOperationException("AssemblyInformationalVersionAttribute not found.");
        }
        string version = attribute.InformationalVersion;
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo {
                Version = $"v{version}",
                Title = "CampingAI API",
                Contact = new OpenApiContact {
                    Name = "Aitor López Palomares",
                    Email = "aitorlopez84@gmail.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                In = ParameterLocation.Header,
                Description = "Por favor, introduce el JWT con la clave \"Bearer\" en el campo indicado para ello.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });


            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      Array.Empty<string>()
                    }
            });

            options.CustomSchemaIds(type => type.ToString());
        });
    }


    private static void ConfigureApp(WebApplication app) {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }



        app.UseMiddleware<Handlers.GlobalExceptionMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapControllers();
    }
}