using Application.Abstractions;
using Application.Services;
using Dapper;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;

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
		
		// Repositories - Factory pattern ile kayıt
		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddScoped<ICategoryRepository, CategoryRepository>();
		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<ICustomerRepository, CustomerRepository>();
		services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
		services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
		services.AddScoped<IPaymentRepository, PaymentRepository>();
		services.AddScoped<IInventoryRepository, InventoryRepository>();
		services.AddScoped<ISellerRepository, SellerRepository>();
		services.AddScoped<IStoreRepository, StoreRepository>();
		services.AddScoped<ICartRepository, CartRepository>();
		services.AddScoped<IOrderItemRepository, OrderItemRepository>();
		services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
		services.AddScoped<IStoreApplicationRepository, StoreApplicationRepository>();
		services.AddScoped<IAppUserRepository, AppUserRepository>();
		
		// Services
		services.AddScoped<IOrderService, OrderService>();
		services.AddScoped<IAppUserService, AppUserService>();
		
		return services;
	}
}
