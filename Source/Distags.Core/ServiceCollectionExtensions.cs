using Distags.Core.Commands;
using Distags.Core.Management;
using Distags.Core.Queries;
using Distags.Core.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Distags.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistagsCore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<TokenGenerator>();

        serviceCollection.AddGenericServiceImplementations(typeof(IQueryHandler<,>));
        serviceCollection.AddGenericServiceImplementations(typeof(ICommandHandler<>));

        return serviceCollection;
    }

    private static IServiceCollection AddGenericServiceImplementations(
        this IServiceCollection serviceCollection, Type genericServiceType)
    {
        IEnumerable<Type> classTypes = typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            .Where(type => !type.IsInterface && !type.IsAbstract);

        foreach (Type classType in classTypes)
        {
            IEnumerable<Type> queryHandlerInterfaces = classType.GetInterfaces()
                .Where(classInterface =>
                    classInterface.IsGenericType &&
                    classInterface.GetGenericTypeDefinition() == genericServiceType);

            foreach (Type queryHandlerInterface in queryHandlerInterfaces)
            {
                serviceCollection.Add(new ServiceDescriptor(queryHandlerInterface, classType, ServiceLifetime.Transient));
            }
        }

        return serviceCollection;
    }
}