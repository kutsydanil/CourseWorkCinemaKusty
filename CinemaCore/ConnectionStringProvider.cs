using Microsoft.Extensions.Configuration;

namespace WebCinema
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString(string connectionStringName)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            return config.GetConnectionString(connectionStringName);
        }
    }
}
