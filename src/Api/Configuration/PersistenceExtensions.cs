using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;

namespace Api.Configuration;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration cfg)
    {
        // Connection string is now handled by SqlConnectionFactory in InfrastructureRegistration
        // No need to register anything here as it's already done in AddInfrastructure()
        return services;
    }
}
