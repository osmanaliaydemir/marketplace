using Infrastructure.Persistence.Naming;
using Domain.Entities;
using Xunit;

namespace Infrastructure.Tests.Persistence.Naming;

public class SnakeCaseTableNameResolverTests
{
    private readonly SnakeCaseTableNameResolver _resolver;

    public SnakeCaseTableNameResolverTests()
    {
        _resolver = new SnakeCaseTableNameResolver();
    }

    [Fact]
    public void ResolveTableName_Category_ShouldReturnCategories()
    {
        // Act
        var result = _resolver.ResolveTableName(typeof(Category));

        // Assert
        Assert.Equal("categories", result);
    }

    [Fact]
    public void ResolveTableName_Product_ShouldReturnProducts()
    {
        // Act
        var result = _resolver.ResolveTableName(typeof(Product));

        // Assert
        Assert.Equal("products", result);
    }

    [Fact]
    public void ResolveTableName_Store_ShouldReturnStores()
    {
        // Act
        var result = _resolver.ResolveTableName(typeof(Store));

        // Assert
        Assert.Equal("stores", result);
    }

    [Fact]
    public void ResolveTableName_AppUser_ShouldReturnAppUsers()
    {
        // Act
        var result = _resolver.ResolveTableName(typeof(AppUser));

        // Assert
        Assert.Equal("app_users", result);
    }
}
