using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLibrary.Infrastructure.NoSQL
{
    internal class Dependencies
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            //var mongoDbSettings = configuration.GetSection("MongoDBSettings");
            //services.Configure<MongoDBSettings>(mongoDbSettings);
        }
    }
}
