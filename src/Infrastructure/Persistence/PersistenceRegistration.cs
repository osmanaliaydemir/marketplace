using Application.Abstractions;
using Application.Services;
using Dapper;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class PersistenceRegistration
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		// Dapper column mapping - snake_case to PascalCase
		DefaultTypeMap.MatchNamesWithUnderscores = true;
		
		// Database Context
		services.AddScoped<IDbContext, DbContext>();
		services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
		
		// Naming Resolvers
		services.AddScoped<ITableNameResolver, SnakeCaseTableNameResolver>();
		services.AddScoped<IColumnNameResolver, SnakeCaseColumnNameResolver>();
		
		// Unit of Work
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IStoreUnitOfWork, StoreUnitOfWork>();
		
		// Repositories
		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddScoped<ICategoryRepository, CategoryRepository>();
		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<ICustomerRepository, CustomerRepository>();
		services.AddScoped<IPaymentRepository, PaymentRepository>();
		services.AddScoped<IInventoryRepository, InventoryRepository>();
		services.AddScoped<ISellerRepository, SellerRepository>();
		services.AddScoped<IStoreRepository, StoreRepository>();
		services.AddScoped<ICartRepository, CartRepository>();
		services.AddScoped<IOrderItemRepository, OrderItemRepository>();
		services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
		services.AddScoped<IOrderService, OrderService>();
		
		// Services
		services.AddScoped<StoreApplicationService>();
		
		return services;
	}
}
