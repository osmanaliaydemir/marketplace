namespace Application.DTOs.Categories;

public sealed record CategoryStatsDto
{
    public int TotalCategories { get; init; }
    public int ActiveCategories { get; init; }
    public int InactiveCategories { get; init; }
    public int FeaturedCategories { get; init; }
    public int RootCategories { get; init; }
    public int SubCategories { get; init; }
    public int CategoriesWithProducts { get; init; }
    public int EmptyCategories { get; init; }
    public decimal AverageProductsPerCategory { get; init; }
    public int MostProductCategoryId { get; init; }
    public string MostProductCategoryName { get; init; } = string.Empty;
    public int MostProductCategoryCount { get; init; }
}
