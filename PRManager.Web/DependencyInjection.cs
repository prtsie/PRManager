using AutoMapper;
using Serilog;
using Serilog.Events;

namespace PRManager.Web;

public static class DependencyInjection
{
    public static void RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddSingleton<IMapper>(provider =>
        {
            var profiles = provider.GetServices<Profile>();
            var mapperConfig = new MapperConfiguration(mc =>
            {
                foreach (var profile in profiles)
                {
                    mc.AddProfile(profile);
                }
            });
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        });
    }

    public static void ConfigureLogger(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();

        services.AddSerilog();
    }
}