using TagTheSpot.Gateway.Extensions;
using TagTheSpot.Gateway.Middleware;

namespace TagTheSpot.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddRoutesConfigurationFiles();

            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseExceptionHandlingMiddleware();

            app.MapReverseProxy();

            app.Run();
        }
    }
}
