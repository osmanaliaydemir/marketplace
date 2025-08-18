using Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public static class ServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddSingleton<IClock, SystemClock>();
		return services;
	}
}

file sealed class SystemClock : IClock
{
	public DateTime UtcNow => DateTime.UtcNow;
}
