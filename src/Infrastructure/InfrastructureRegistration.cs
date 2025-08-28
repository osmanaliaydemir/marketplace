using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Infrastructure.Payments;
using Infrastructure.Services;
using Infrastructure.Caching;
using Infrastructure.Logging;
using Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureRegistration
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// Persistence
		services.AddPersistence(configuration);
		
		// Payments
		services.AddScoped<Infrastructure.Payments.IPaymentProvider, PaytrAdapter>();
		services.AddScoped<Application.Abstractions.IPaymentProvider, PaymentProvider>();
		services.AddHttpClient<PaytrAdapter>();
		
		// Email Service
		services.AddScoped<IEmailService, EmailService>();
		
		// Password Reset Service
		services.AddScoped<IPasswordResetService, PasswordResetService>();
		
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
