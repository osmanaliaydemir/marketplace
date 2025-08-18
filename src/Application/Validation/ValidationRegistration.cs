using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Validation;

public static class ValidationRegistration
{
	public static IServiceCollection AddValidation(this IServiceCollection services)
	{
		services.AddValidatorsFromAssemblyContaining<StoreApplicationCreateRequestValidator>();
		return services;
	}
}
