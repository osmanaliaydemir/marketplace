using Application.DTOs.Stores;
using Application.DTOs.Products;
using Application.DTOs.Categories;
using Application.DTOs.Cart;
using Application.DTOs.Orders;
using Application.DTOs.Payments;
using Application.DTOs.Inventory;
using Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Validation;

public static class ValidationRegistration
{
	public static IServiceCollection AddApplicationValidation(this IServiceCollection services)
	{
		// Store Application Validators
		services.AddScoped<IValidator<StoreApplicationCreateRequest>, StoreApplicationCreateRequestValidator>();
		
		// Product Validators
		services.AddScoped<IValidator<ProductCreateRequest>, ProductCreateRequestValidator>();
		services.AddScoped<IValidator<ProductUpdateRequest>, ProductUpdateRequestValidator>();
		
		// Category Validators
		services.AddScoped<IValidator<CategoryCreateRequest>, CategoryCreateRequestValidator>();
		services.AddScoped<IValidator<CategoryUpdateRequest>, CategoryUpdateRequestValidator>();
		
		// Cart Validators
		services.AddScoped<IValidator<CartAddItemRequest>, CartAddItemRequestValidator>();
		
		// Order Validators
		services.AddScoped<IValidator<OrderCreateRequest>, OrderCreateRequestValidator>();
		
		// Payment Validators
		services.AddScoped<IValidator<PaymentInitiationRequest>, PaymentInitiationRequestValidator>();
		
		// Inventory Validators
		services.AddScoped<IValidator<StockUpdateRequest>, StockUpdateRequestValidator>();
		
		return services;
	}
}
