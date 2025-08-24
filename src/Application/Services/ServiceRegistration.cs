using Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public static class ServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddSingleton<IClock, SystemClock>();
		
		// Service interfaces registration
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<ICategoryService, CategoryService>();
		services.AddScoped<IOrderService, OrderService>();
		services.AddScoped<ICartService, CartService>();
		services.AddScoped<IPaymentService, PaymentService>();
		services.AddScoped<IInventoryService, InventoryService>();
		services.AddScoped<IExceptionLogService, ExceptionLogService>();

		// Business Rule Validators
		services.AddScoped<BusinessRuleValidator>();
		
		return services;
	}
}

file sealed class SystemClock : IClock
{
	public DateTime UtcNow => DateTime.UtcNow;
}
