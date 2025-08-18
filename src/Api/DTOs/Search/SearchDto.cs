namespace Api.DTOs.Search;

public class SearchRequestDto
{
    public string Query { get; set; } = string.Empty;
    public long? StoreId { get; set; }
    public long? CategoryId { get; set; }
    public long? StoreCategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } // "price", "name", "created_at"
    public string? SortOrder { get; set; } // "asc", "desc"
}

public class SearchResultDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<SearchProductDto> Products { get; set; } = new();
    public List<SearchFacetDto> Facets { get; set; } = new();
}

public class SearchProductDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Store information
    public long StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    
    // Category information
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CategorySlug { get; set; }
    
    // Search relevance
    public double RelevanceScore { get; set; }
    public List<string> MatchedTerms { get; set; } = new();
}

public class SearchFacetDto
{
    public string Field { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<FacetValueDto> Values { get; set; } = new();
}

public class FacetValueDto
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

public class SearchSuggestionDto
{
    public string Suggestion { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "product", "category", "store"
    public long? Id { get; set; }
    public string? Slug { get; set; }
    public int Relevance { get; set; }
}
