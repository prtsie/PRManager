using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PRManager.Common.Core.Attributes;
using PRManager.Common.Core.Models;

namespace PRManager.Common.Mvc.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует все интерфейсы инстансов в указанной сборке для указанного маркерного интерфейса
    /// </summary>
    public static void RegisterAssemblyInterfacesAssignableTo<TInterface>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        var serviceType = typeof(TInterface);
        var types = serviceType.Assembly.GetTypes()
            .Where(p => serviceType.IsAssignableFrom(p) &&
                        !(p.IsAbstract ||
                          p.IsInterface));
        foreach (var type in types)
        {
            services.TryAdd(new ServiceDescriptor(type, type, lifetime));
            var interfaces = type.GetTypeInfo()
                .ImplementedInterfaces
                .Where(i => i != typeof(IDisposable) &&
                            i.IsPublic &&
                            i != serviceType);

            foreach (var interfaceType in interfaces)
            {
                services.TryAdd(new ServiceDescriptor(interfaceType,
                    provider => provider.GetRequiredService(type),
                    lifetime));
            }
        }
    }

    public static void RegisterImplementationsOf<TService>(this IServiceCollection services,
        Assembly implementationsAssembly,
        ServiceLifetime lifetime)
    {
        var serviceType = typeof(TService);
        
        services.RemoveAll<TService>();

        var types = implementationsAssembly.GetTypes()
            .Where(t => t != serviceType &&
                        serviceType.IsAssignableFrom(t) &&
                        t is
                        {
                            IsAbstract: false,
                            IsPublic: true,
                            IsInterface: false
                        })
            .OrderByDescending(t =>
            {
                var attribute = t.GetCustomAttribute<RegisterPriorityAttribute>();
                var priority = attribute?.Priority ?? RegisterPriorities.Default;
                return (int)priority;
            });
        
        services.TryAddEnumerable(types.Select(x => new ServiceDescriptor(serviceType, x, lifetime)));
    }

    public static void RegisterAutoMapperProfile<T>(this IServiceCollection services) where T : Profile 
        => services.AddSingleton<Profile, T>();
}