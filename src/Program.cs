using TagTheSpot.Gateway.Extensions;
using TagTheSpot.Services.Shared.API.DependencyInjection;
using TagTheSpot.Services.Shared.API.Middleware;
using TagTheSpot.Services.Shared.Application.Extensions;
using TagTheSpot.Services.Shared.Auth.DependencyInjection;
using TagTheSpot.Services.Shared.Auth.Options;

namespace TagTheSpot.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddRoutesConfigurationFiles();

            builder.Services.ConfigureValidatableOnStartOptions<JwtSettings>();

            builder.Services.ConfigureAuthentication();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", options =>
                {
                    options
                        .RequireAuthenticatedUser();
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

            builder.Services.AddDevelopmentCorsPolicy();

            builder.OverrideClusterUrl("user-cluster", "USER_SERVICE_URL")
                   .OverrideClusterUrl("spot-cluster", "SPOT_SERVICE_URL")
                   .OverrideClusterUrl("moderation-cluster", "MODERATION_SERVICE_URL");

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseCors(CorsExtensions.DevelopmentPolicyName);
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseExceptionHandlingMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapReverseProxy();

            app.Run();
        }
    }
}
