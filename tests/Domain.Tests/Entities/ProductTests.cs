using Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace Domain.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void Product_WithValidProperties_ShouldSetValuesCorrectly()
    {
        // Arrange & Act
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StoreId = 1L,
            CategoryId = 1L,
            SellerId = 1L,
            Currency = "TRY",
            StockQty = 100,
            IsActive = true,
            IsFeatured = false,
            IsPublished = true,
            Weight = 500m
        };

        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Test Description");
        product.Price.Should().Be(99.99m);
        product.StoreId.Should().Be(1L);
        product.CategoryId.Should().Be(1L);
        product.SellerId.Should().Be(1L);
        product.Currency.Should().Be("TRY");
        product.StockQty.Should().Be(100);
        product.IsActive.Should().BeTrue();
        product.IsFeatured.Should().BeFalse();
        product.IsPublished.Should().BeTrue();
        product.Weight.Should().Be(500m);
    }

    [Fact]
    public void Product_WithDefaultValues_ShouldHaveCorrectDefaults()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.Name.Should().Be(string.Empty);
        product.Slug.Should().Be(string.Empty);
        product.Sku.Should().Be(string.Empty);
        product.Currency.Should().Be("TRY");
        product.StockQty.Should().Be(0);
        product.IsActive.Should().BeTrue();
        product.IsFeatured.Should().BeFalse();
        product.IsPublished.Should().BeTrue();
        product.DisplayOrder.Should().Be(0);
        product.Weight.Should().Be(0m);
        product.MinOrderQty.Should().Be(1);
        product.MaxOrderQty.Should().Be(999);
    }

    [Fact]
    public void Product_Collections_ShouldBeInitialized()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.Variants.Should().NotBeNull();
        product.Images.Should().NotBeNull();
        product.Variants.Should().BeEmpty();
        product.Images.Should().BeEmpty();
    }

    [Fact]
    public void Product_WithNullDescription_ShouldHandleNullValues()
    {
        // Arrange & Act
        var product = new Product
        {
            Description = null,
            ShortDescription = null,
            MetaTitle = null,
            MetaDescription = null,
            MetaKeywords = null
        };

        // Assert
        product.Description.Should().BeNull();
        product.ShortDescription.Should().BeNull();
        product.MetaTitle.Should().BeNull();
        product.MetaDescription.Should().BeNull();
        product.MetaKeywords.Should().BeNull();
    }

    [Fact]
    public void Product_WithSpecialCharacters_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var specialName = "Test Ürün - Özel Karakterler & Semboller";
        var specialDescription = "Açıklama: Türkçe karakterler, özel semboller @#$%^&*()";

        // Act
        var product = new Product
        {
            Name = specialName,
            Description = specialDescription
        };

        // Assert
        product.Name.Should().Be(specialName);
        product.Description.Should().Be(specialDescription);
    }

    [Fact]
    public void Product_WithHighPrice_ShouldHandleLargeValues()
    {
        // Arrange
        var highPrice = 999999.99m;
        var highStock = int.MaxValue;

        // Act
        var product = new Product
        {
            Price = highPrice,
            StockQty = highStock
        };

        // Assert
        product.Price.Should().Be(highPrice);
        product.StockQty.Should().Be(highStock);
    }

    [Fact]
    public void Product_WithZeroPrice_ShouldHandleZeroValues()
    {
        // Arrange & Act
        var product = new Product
        {
            Price = 0m,
            StockQty = 0,
            Weight = 0m
        };

        // Assert
        product.Price.Should().Be(0m);
        product.StockQty.Should().Be(0);
        product.Weight.Should().Be(0m);
    }
}
