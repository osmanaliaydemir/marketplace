using System;
using System.Threading.Tasks;
using Xunit;
using Application.DTOs.Products;
using Application.Services;
using Moq;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Api.Tests.Controllers;

public class ProductCreateUnitTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly Mock<ISellerRepository> _sellerRepositoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _productService;

    public ProductCreateUnitTest()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _storeRepositoryMock = new Mock<IStoreRepository>();
        _sellerRepositoryMock = new Mock<ISellerRepository>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _productService = new ProductService(
            _loggerMock.Object,
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _storeRepositoryMock.Object,
            _sellerRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ShouldReturnProductDetail()
    {
        // Arrange
        var categoryId = 1L;
        var storeId = 1L;
        var sellerId = 1L;

        var category = new Category
        {
            Id = categoryId,
            Name = "Test Kategori",
            Slug = "test-kategori",
            IsActive = true
        };

        var store = new Store
        {
            Id = storeId,
            Name = "Test Mağaza",
            Slug = "test-magaza",
            IsActive = true
        };

        var seller = new Seller
        {
            Id = sellerId,
            UserId = 1L,
            CommissionRate = 5.0m,
            IsActive = true
        };

        var productRequest = new ProductCreateRequest
        {
            Name = "Test Ürün",
            Description = "Test ürün açıklaması",
            ShortDescription = "Kısa açıklama",
            Sku = "TEST-SKU-001",
            CategoryId = categoryId,
            StoreId = storeId,
            SellerId = sellerId,
            Price = 99.99m,
            CompareAtPrice = 149.99m,
            Currency = "TRY",
            StockQty = 10,
            Weight = 500.0m,
            MinOrderQty = 1,
            MaxOrderQty = 10,
            DisplayOrder = 0,
            MetaTitle = "Test Ürün Meta",
            MetaDescription = "Test ürün meta açıklaması",
            IsActive = true,
            IsFeatured = false,
            IsPublished = true
        };

        var createdProduct = new Product
        {
            Id = 1L,
            Name = productRequest.Name,
            Description = productRequest.Description,
            ShortDescription = productRequest.ShortDescription,
            Sku = productRequest.Sku,
            CategoryId = productRequest.CategoryId,
            StoreId = productRequest.StoreId,
            SellerId = productRequest.SellerId,
            Price = productRequest.Price,
            CompareAtPrice = productRequest.CompareAtPrice,
            Currency = productRequest.Currency,
            StockQty = productRequest.StockQty,
            Weight = productRequest.Weight,
            MinOrderQty = productRequest.MinOrderQty ?? 1,
            MaxOrderQty = productRequest.MaxOrderQty ?? 999,
            DisplayOrder = productRequest.DisplayOrder,
            MetaTitle = productRequest.MetaTitle,
            MetaDescription = productRequest.MetaDescription,
            IsActive = productRequest.IsActive,
            IsFeatured = productRequest.IsFeatured,
            IsPublished = productRequest.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _storeRepositoryMock.Setup(x => x.GetByIdAsync(storeId))
            .ReturnsAsync(store);
        _productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateAsync(productRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productRequest.Name, result.Name);
        Assert.Equal(productRequest.Sku, result.Sku);
        Assert.Equal(productRequest.Price, result.Price);
        Assert.Equal(productRequest.CategoryId, result.CategoryId);
        Assert.Equal(productRequest.StoreId, result.StoreId);
        Assert.Equal(productRequest.SellerId, result.SellerId);

        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(categoryId), Times.Exactly(2)); // CreateAsync + MapToProductDetailDtoAsync
        _storeRepositoryMock.Verify(x => x.GetByIdAsync(storeId), Times.Exactly(2)); // CreateAsync + MapToProductDetailDtoAsync
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidCategory_ShouldThrowArgumentException()
    {
        // Arrange
        var categoryId = 999L;
        var storeId = 1L;
        var sellerId = 1L;

        var productRequest = new ProductCreateRequest
        {
            Name = "Test Ürün",
            Description = "Test ürün açıklaması",
            Sku = "TEST-SKU-002",
            CategoryId = categoryId,
            StoreId = storeId,
            SellerId = sellerId,
            Price = 99.99m,
            Currency = "TRY",
            StockQty = 10
        };

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _productService.CreateAsync(productRequest));

        Assert.Contains($"Category with ID {categoryId} not found", exception.Message);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidStore_ShouldThrowArgumentException()
    {
        // Arrange
        var categoryId = 1L;
        var storeId = 999L;
        var sellerId = 1L;

        var category = new Category
        {
            Id = categoryId,
            Name = "Test Kategori",
            Slug = "test-kategori",
            IsActive = true
        };

        var productRequest = new ProductCreateRequest
        {
            Name = "Test Ürün",
            Description = "Test ürün açıklaması",
            Sku = "TEST-SKU-003",
            CategoryId = categoryId,
            StoreId = storeId,
            SellerId = sellerId,
            Price = 99.99m,
            Currency = "TRY",
            StockQty = 10
        };

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _storeRepositoryMock.Setup(x => x.GetByIdAsync(storeId))
            .ReturnsAsync((Store?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _productService.CreateAsync(productRequest));

        Assert.Contains($"Store with ID {storeId} not found", exception.Message);
        _categoryRepositoryMock.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
        _storeRepositoryMock.Verify(x => x.GetByIdAsync(storeId), Times.Once);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
    }
}
