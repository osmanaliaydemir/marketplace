using Application.Abstractions;
using Application.DTOs.Products;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests.Persistence;

public class ProductRepositoryTests
{
    private readonly Mock<ILogger<ProductRepository>> _mockLogger;

    public ProductRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<ProductRepository>>();
    }

    [Fact]
    public void ProductRepository_ShouldBeCreated()
    {
        // Arrange & Act
        var repository = CreateRepository();

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        var action = () => repository.GetByIdAsync(1L);
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetAllAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        var action = () => repository.GetAllAsync();
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task AddAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();
        var product = new Product { Name = "Test Product" };

        // Act & Assert
        var action = () => repository.AddAsync(product);
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task UpdateAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();
        var product = new Product { Id = 1L, Name = "Test Product" };

        // Act & Assert
        var action = () => repository.UpdateAsync(product);
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task DeleteAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        var action = () => repository.DeleteAsync(1L);
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetActiveProductsAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        var action = () => repository.GetActiveProductsAsync();
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetPublishedProductsAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        var action = () => repository.GetPublishedProductsAsync();
        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetFeaturedProductsAsync_WithNullContext_ShouldThrowException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        var action = () => repository.GetFeaturedProductsAsync();
        await action.Should().ThrowAsync<Exception>();
    }

    private ProductRepository CreateRepository()
    {
        // This is a simplified repository creation for testing purposes
        // In a real scenario, you would use proper dependency injection and DbContext
        var mockContext = new Mock<Infrastructure.Persistence.Context.IDbContext>();
        var mockTableNameResolver = new Mock<Infrastructure.Persistence.Naming.ITableNameResolver>();
        var mockColumnNameResolver = new Mock<Infrastructure.Persistence.Naming.IColumnNameResolver>();
        
        return new ProductRepository(
            mockContext.Object, 
            _mockLogger.Object, 
            mockTableNameResolver.Object, 
            mockColumnNameResolver.Object);
    }
}
