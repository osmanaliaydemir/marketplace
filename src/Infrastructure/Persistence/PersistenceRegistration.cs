using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

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
		services.AddScoped<Application.Abstractions.IProductRepository, ProductRepository>();
		services.AddScoped<Application.Abstractions.ICategoryRepository, CategoryRepository>();
		services.AddScoped<Application.Abstractions.IOrderRepository, OrderRepository>();
		services.AddScoped<Application.Abstractions.ICustomerRepository, CustomerRepository>();
		services.AddScoped<Application.Abstractions.IPaymentRepository, PaymentRepository>();
		
		// Services
		services.AddScoped<StoreApplicationService>();
		
		return services;
	}
}
