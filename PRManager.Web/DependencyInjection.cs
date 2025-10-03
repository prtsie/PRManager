using AutoMapper;

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
}