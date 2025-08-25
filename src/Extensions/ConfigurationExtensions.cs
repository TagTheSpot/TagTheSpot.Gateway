namespace TagTheSpot.Gateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddRoutesConfigurationFiles(
            this IConfigurationBuilder builder)
        {
            builder.AddJsonFile("Configurations/clusters.json", optional: true, reloadOnChange: true);
            builder.AddJsonFile("Configurations/user.routes.json", optional: true, reloadOnChange: true);
            builder.AddJsonFile("Configurations/spot.routes.json", optional: true, reloadOnChange: true);
            builder.AddJsonFile("Configurations/moderation.routes.json", optional: true, reloadOnChange: true);

            return builder;
        }
    }
}
