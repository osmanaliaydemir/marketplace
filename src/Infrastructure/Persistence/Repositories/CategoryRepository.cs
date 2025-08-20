using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;

namespace Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CategoryRepository(
        IDbContext context,
        ILogger<CategoryRepository> logger,
        ISqlConnectionFactory connectionFactory) 
        : base(context, logger, null, null)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        _logger.LogInformation("Getting category by slug: {Slug}", slug);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT Id, Name, Description, Slug, ParentId, DisplayOrder, 
                       IsActive, IsFeatured, MetaTitle, MetaDescription, 
                       CreatedAt, ModifiedAt
                FROM Categories 
                WHERE Slug = @Slug AND IsDeleted = 0";
            
            var category = await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Slug = slug });
            
            _logger.LogInformation("Category found by slug: {Slug}, Id: {Id}", slug, category?.Id);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by slug: {Slug}", slug);
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        _logger.LogInformation("Getting root categories");
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT Id, Name, Description, Slug, ParentId, DisplayOrder, 
                       IsActive, IsFeatured, MetaTitle, MetaDescription, 
                       CreatedAt, ModifiedAt
                FROM Categories 
                WHERE ParentId IS NULL AND IsActive = 1 AND IsDeleted = 0
                ORDER BY DisplayOrder, Name";
            
            var categories = await connection.QueryAsync<Category>(sql);
            
            _logger.LogInformation("Found {Count} root categories", categories.Count());
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting root categories");
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(long parentId)
    {
        _logger.LogInformation("Getting sub categories for parent: {ParentId}", parentId);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT Id, Name, Description, Slug, ParentId, DisplayOrder, 
                       IsActive, IsFeatured, MetaTitle, MetaDescription, 
                       CreatedAt, ModifiedAt
                FROM Categories 
                WHERE ParentId = @ParentId AND IsActive = 1 AND IsDeleted = 0
                ORDER BY DisplayOrder, Name";
            
            var categories = await connection.QueryAsync<Category>(sql, new { ParentId = parentId });
            
            _logger.LogInformation("Found {Count} sub categories for parent {ParentId}", categories.Count(), parentId);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sub categories for parent: {ParentId}", parentId);
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        _logger.LogInformation("Getting active categories");
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT Id, Name, Description, Slug, ParentId, DisplayOrder, 
                       IsActive, IsFeatured, MetaTitle, MetaDescription, 
                       CreatedAt, ModifiedAt
                FROM Categories 
                WHERE IsActive = 1 AND IsDeleted = 0
                ORDER BY DisplayOrder, Name";
            
            var categories = await connection.QueryAsync<Category>(sql);
            
            _logger.LogInformation("Found {Count} active categories", categories.Count());
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active categories");
            throw;
        }
    }

    public async Task<IEnumerable<Category>> GetFeaturedCategoriesAsync()
    {
        _logger.LogInformation("Getting featured categories");
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT Id, Name, Description, Slug, ParentId, DisplayOrder, 
                       IsActive, IsFeatured, MetaTitle, MetaDescription, 
                       CreatedAt, ModifiedAt
                FROM Categories 
                WHERE IsFeatured = 1 AND IsActive = 1 AND IsDeleted = 0
                ORDER BY DisplayOrder, Name";
            
            var categories = await connection.QueryAsync<Category>(sql);
            
            _logger.LogInformation("Found {Count} featured categories", categories.Count());
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured categories");
            throw;
        }
    }

    public async Task<int> GetProductCountAsync(long categoryId)
    {
        _logger.LogInformation("Getting product count for category: {CategoryId}", categoryId);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(*) 
                FROM Products 
                WHERE CategoryId = @CategoryId AND IsActive = 1 AND IsDeleted = 0";
            
            var count = await connection.ExecuteScalarAsync<int>(sql, new { CategoryId = categoryId });
            
            _logger.LogInformation("Category {CategoryId} has {Count} products", categoryId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count for category: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(long categoryId)
    {
        _logger.LogInformation("Checking if category has children: {CategoryId}", categoryId);
        
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(*) 
                FROM Categories 
                WHERE ParentId = @CategoryId AND IsDeleted = 0";
            
            var count = await connection.ExecuteScalarAsync<int>(sql, new { CategoryId = categoryId });
            var hasChildren = count > 0;
            
            _logger.LogInformation("Category {CategoryId} has children: {HasChildren}", categoryId, hasChildren);
            return hasChildren;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category has children: {CategoryId}", categoryId);
            throw;
        }
    }
}
