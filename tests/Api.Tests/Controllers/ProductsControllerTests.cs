using Api.Controllers;
using Application.Abstractions;
using Application.DTOs.Products;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        
        _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
    }

    [Fact]
    public void ProductsController_ShouldBeCreated()
    {
        // Arrange & Act
        var controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);

        // Assert
        controller.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProduct_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var productId = 1L;
        var product = new Application.DTOs.Products.ProductDetailDto { Id = productId, Name = "Test Product" };
        
        _mockProductService.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Should().BeOfType<ActionResult<Application.DTOs.Products.ProductDetailDto>>().Subject;
        actionResult.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var productId = 999L;
        _mockProductService.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Application.DTOs.Products.ProductDetailDto?)null);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Should().BeOfType<ActionResult<Application.DTOs.Products.ProductDetailDto>>().Subject;
        actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateProduct_WithValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = new Application.DTOs.Products.ProductCreateRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            CategoryId = 1L,
            StoreId = 1L,
            SellerId = 1L
        };
        
        var createdProduct = new Application.DTOs.Products.ProductDetailDto { Id = 1L, Name = "Test Product" };
        
        _mockProductService.Setup(x => x.CreateAsync(request))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Should().BeOfType<ActionResult<Application.DTOs.Products.ProductDetailDto>>().Subject;
        actionResult.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task UpdateProduct_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var productId = 1L;
        var request = new Application.DTOs.Products.ProductUpdateRequest
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 149.99m,
            CategoryId = 1L
        };
        
        var updatedProduct = new Application.DTOs.Products.ProductDetailDto { Id = productId, Name = "Updated Product" };
        
        _mockProductService.Setup(x => x.UpdateAsync(productId, request))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(productId, request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Should().BeOfType<ActionResult<Application.DTOs.Products.ProductDetailDto>>().Subject;
        actionResult.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PublishProduct_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var productId = 1L;
        _mockProductService.Setup(x => x.PublishAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.PublishProduct(productId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UnpublishProduct_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var productId = 1L;
        _mockProductService.Setup(x => x.UnpublishAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UnpublishProduct(productId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
    }
}
