using Application.Abstractions;
using Application.DTOs.Products;
using Application.Services;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly Mock<ISellerRepository> _mockSellerRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockStoreRepository = new Mock<IStoreRepository>();
        _mockSellerRepository = new Mock<ISellerRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        
        _productService = new ProductService(
            _mockLogger.Object,
            _mockProductRepository.Object,
            _mockCategoryRepository.Object,
            _mockStoreRepository.Object,
            _mockSellerRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1L;
        var product = new Product { Id = productId, Name = "Test Product" };
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        _mockProductRepository.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var productId = 999L;
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().BeNull();
        _mockProductRepository.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateProduct()
    {
        // Arrange
        var request = new ProductCreateRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            CategoryId = 1L,
            StoreId = 1L,
            SellerId = 1L
        };
        
        var category = new Category { Id = 1L, Name = "Test Category" };
        var store = new Store { Id = 1L, Name = "Test Store" };
        var product = new Product { Id = 1L, Name = "Test Product" };
        
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);
        _mockStoreRepository.Setup(x => x.GetByIdAsync(request.StoreId))
            .ReturnsAsync(store);
        _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockProductRepository.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCategoryId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new ProductCreateRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            CategoryId = 1L,
            StoreId = 1L,
            SellerId = 1L
        };
        
        _mockCategoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        var action = () => _productService.CreateAsync(request);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldUpdateProduct()
    {
        // Arrange
        var productId = 1L;
        var request = new ProductUpdateRequest
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 149.99m,
            CategoryId = 1L
        };
        
        var existingProduct = new Product { Id = productId, Name = "Original Product" };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _productService.UpdateAsync(productId, request);

        // Assert
        result.Should().NotBeNull();
        _mockProductRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentProduct_ShouldThrowArgumentException()
    {
        // Arrange
        var productId = 999L;
        var request = new ProductUpdateRequest
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 149.99m,
            CategoryId = 1L
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        var action = () => _productService.UpdateAsync(productId, request);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteProduct()
    {
        // Arrange
        var productId = 1L;
        var existingProduct = new Product { Id = productId, Name = "Test Product" };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();
        _mockProductRepository.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentProduct_ShouldReturnFalse()
    {
        // Arrange
        var productId = 999L;
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeFalse();
        _mockProductRepository.Verify(x => x.DeleteAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task ListAsync_WithValidRequest_ShouldReturnProducts()
    {
        // Arrange
        var request = new ProductListRequest { Page = 1, PageSize = 10 };
        var products = new List<Product>
        {
            new Product { Id = 1L, Name = "Product 1" },
            new Product { Id = 2L, Name = "Product 2" }
        };

        _mockProductRepository.Setup(x => x.GetPublishedProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _productService.ListAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockProductRepository.Verify(x => x.GetPublishedProductsAsync(), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithValidRequest_ShouldReturnSearchResults()
    {
        // Arrange
        var request = new ProductSearchRequest
        {
            SearchTerm = "test",
            Page = 1,
            PageSize = 10
        };
        
        var products = new List<Product>
        {
            new Product { Id = 1L, Name = "Test Product 1" },
            new Product { Id = 2L, Name = "Test Product 2" }
        };

        _mockProductRepository.Setup(x => x.SearchProductsAsync(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.SearchAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockProductRepository.Verify(x => x.SearchProductsAsync(It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<long?>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithValidId_ShouldPublishProduct()
    {
        // Arrange
        var productId = 1L;
        var existingProduct = new Product { Id = productId, Name = "Test Product" };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _productService.PublishAsync(productId);

        // Assert
        result.Should().BeTrue();
        _mockProductRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task SetActiveAsync_WithValidId_ShouldSetActive()
    {
        // Arrange
        var productId = 1L;
        var isActive = true;
        var existingProduct = new Product { Id = productId, Name = "Test Product" };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _productService.SetActiveAsync(productId, isActive);

        // Assert
        result.Should().BeTrue();
        _mockProductRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }
}
