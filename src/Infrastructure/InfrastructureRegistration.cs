using Infrastructure.Persistence;
using Infrastructure.Payments;
using Infrastructure.Caching;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureRegistration
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// Persistence
		services.AddPersistence(configuration);
		
		// Payments
		services.AddScoped<IPaymentProvider, PaytrAdapter>();
		services.AddHttpClient<PaytrAdapter>();
		
		// Caching
		services.AddSingleton<IConnectionMultiplexer>(provider =>
		{
			var connectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
			return ConnectionMultiplexer.Connect(connectionString);
		});
		services.AddScoped<ICacheService, RedisCacheService>();
		
		// Logging
		services.AddLogging();
		
		return services;
	}
}
