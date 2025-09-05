using TagTheSpot.Gateway.Extensions;
using TagTheSpot.Gateway.Middleware;
using TagTheSpot.Gateway.Options;

namespace TagTheSpot.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddRoutesConfigurationFiles();

            builder.Services.AddOptions<JwtSettings>()
                .BindConfiguration(JwtSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.ConfigureAuthentication();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", options =>
                {
                    options.RequireAuthenticatedUser();
                });

                options.AddPolicy("Admin", options =>
                {
                    options
                        .RequireAuthenticatedUser()
                        .RequireRole("Admin", "Owner");
                });

                options.AddPolicy("Owner", options =>
                {
                    options
                        .RequireAuthenticatedUser()
                        .RequireRole("Owner");
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DEV", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseCors("DEV");
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandlingMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapReverseProxy();

            app.Run();
        }
    }
}
