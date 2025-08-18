using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Naming;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace Infrastructure;

public static class InfrastructureRegistration
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		// Dapper column mapping - snake_case to PascalCase
		DefaultTypeMap.MatchNamesWithUnderscores = true;
		
		// Persistence
		services.AddScoped<IDbContext, DbContext>();
		services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
		services.AddScoped<ITableNameResolver, SnakeCaseTableNameResolver>();
		services.AddScoped<IColumnNameResolver, SnakeCaseColumnNameResolver>();
		
		// Unit of Work
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IStoreUnitOfWork, StoreUnitOfWork>();
		
		// Services
		services.AddScoped<StoreApplicationService>();
		
		return services;
	}
}
