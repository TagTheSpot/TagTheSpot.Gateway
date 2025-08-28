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

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseExceptionHandlingMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapReverseProxy();

            app.Run();
        }
    }
}
