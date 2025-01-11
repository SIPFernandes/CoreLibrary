using CoreLibrary.Core.Interfaces;
using CoreLibrary.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLibrary
{
    public class Dependencies
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped(typeof(IGenericService<,,>), typeof(GenericService<,,>));
        }
    }
}
