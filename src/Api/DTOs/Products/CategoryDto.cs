using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Products;

/// <summary>
/// Kategori bilgilerini içeren DTO
/// </summary>
public sealed class CategoryDto
{
	/// <summary>
	/// Kategori benzersiz kimliği
	/// </summary>
	/// <example>1</example>
	public long Id { get; set; }

	/// <summary>
	/// Üst kategori ID'si (null ise ana kategori)
	/// </summary>
	/// <example>null</example>
	public long? ParentId { get; set; }

	/// <summary>
	/// Kategori adı
	/// </summary>
	/// <example>Elektronik</example>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// SEO dostu URL slug'ı
	/// </summary>
	/// <example>elektronik</example>
	public string? Slug { get; set; }

	/// <summary>
	/// Kategori açıklaması
	/// </summary>
	/// <example>Elektronik ürünler kategorisi</example>
	public string? Description { get; set; }

	/// <summary>
	/// Kategori resim URL'si
	/// </summary>
	/// <example>https://example.com/images/electronics.jpg</example>
	public string? ImageUrl { get; set; }

	/// <summary>
	/// Bootstrap icon sınıfı
	/// </summary>
	/// <example>bi-phone</example>
	public string? IconClass { get; set; }

	/// <summary>
	/// Kategori aktif mi?
	/// </summary>
	/// <example>true</example>
	public bool IsActive { get; set; }

	/// <summary>
	/// Ana sayfada gösterilecek mi?
	/// </summary>
	/// <example>false</example>
	public bool IsFeatured { get; set; }

	/// <summary>
	/// Görüntüleme sırası
	/// </summary>
	/// <example>1</example>
	public int DisplayOrder { get; set; }

	/// <summary>
	/// SEO meta başlık
	/// </summary>
	/// <example>Elektronik Ürünler - Online Mağaza</example>
	public string? MetaTitle { get; set; }

	/// <summary>
	/// SEO meta açıklama
	/// </summary>
	/// <example>En yeni elektronik ürünler, telefon, bilgisayar ve daha fazlası</example>
	public string? MetaDescription { get; set; }

	/// <summary>
	/// Oluşturulma tarihi
	/// </summary>
	/// <example>2024-01-15T10:30:00Z</example>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Son güncellenme tarihi
	/// </summary>
	/// <example>2024-01-20T14:45:00Z</example>
	public DateTime? ModifiedAt { get; set; }
	
	/// <summary>
	/// Üst kategori bilgisi
	/// </summary>
	public CategoryDto? Parent { get; set; }

	/// <summary>
	/// Alt kategoriler listesi
	/// </summary>
	public List<CategoryDto> Children { get; set; } = new();

	/// <summary>
	/// Bu kategorideki ürün sayısı
	/// </summary>
	/// <example>25</example>
	public int ProductCount { get; set; } = 0;
}

/// <summary>
/// Yeni kategori oluşturma isteği
/// </summary>
public sealed class CreateCategoryRequest
{
	/// <summary>
	/// Üst kategori ID'si (null ise ana kategori)
	/// </summary>
	/// <example>null</example>
	public long? ParentId { get; set; }

	/// <summary>
	/// Kategori adı (zorunlu)
	/// </summary>
	/// <example>Elektronik</example>
	[Required(ErrorMessage = "Kategori adı gereklidir")]
	[StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// SEO dostu URL slug'ı (otomatik oluşturulur)
	/// </summary>
	/// <example>elektronik</example>
	[StringLength(150, ErrorMessage = "Slug en fazla 150 karakter olabilir")]
	public string? Slug { get; set; }

	/// <summary>
	/// Kategori açıklaması
	/// </summary>
	/// <example>Elektronik ürünler kategorisi</example>
	[StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
	public string? Description { get; set; }

	/// <summary>
	/// Kategori resim URL'si
	/// </summary>
	/// <example>https://example.com/images/electronics.jpg</example>
	[StringLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir")]
	[Url(ErrorMessage = "Geçerli bir URL giriniz")]
	public string? ImageUrl { get; set; }

	/// <summary>
	/// Bootstrap icon sınıfı
	/// </summary>
	/// <example>bi-phone</example>
	[StringLength(50, ErrorMessage = "İkon sınıfı en fazla 50 karakter olabilir")]
	public string? IconClass { get; set; }

	/// <summary>
	/// Kategori aktif mi?
	/// </summary>
	/// <example>true</example>
	public bool IsActive { get; set; } = true;

	/// <summary>
	/// Ana sayfada gösterilecek mi?
	/// </summary>
	/// <example>false</example>
	public bool IsFeatured { get; set; } = false;

	/// <summary>
	/// Görüntüleme sırası
	/// </summary>
	/// <example>1</example>
	[Range(0, 999, ErrorMessage = "Görüntüleme sırası 0-999 arasında olmalıdır")]
	public int DisplayOrder { get; set; } = 0;

	/// <summary>
	/// SEO meta başlık
	/// </summary>
	/// <example>Elektronik Ürünler - Online Mağaza</example>
	[StringLength(60, ErrorMessage = "Meta başlık en fazla 60 karakter olabilir")]
	public string? MetaTitle { get; set; }

	/// <summary>
	/// SEO meta açıklama
	/// </summary>
	/// <example>En yeni elektronik ürünler, telefon, bilgisayar ve daha fazlası</example>
	[StringLength(160, ErrorMessage = "Meta açıklama en fazla 160 karakter olabilir")]
	public string? MetaDescription { get; set; }
}

/// <summary>
/// Kategori güncelleme isteği
/// </summary>
public sealed class UpdateCategoryRequest
{
	/// <summary>
	/// Üst kategori ID'si (null ise ana kategori)
	/// </summary>
	/// <example>null</example>
	public long? ParentId { get; set; }

	/// <summary>
	/// Kategori adı (zorunlu)
	/// </summary>
	/// <example>Elektronik</example>
	[Required(ErrorMessage = "Kategori adı gereklidir")]
	[StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// SEO dostu URL slug'ı (otomatik oluşturulur)
	/// </summary>
	/// <example>elektronik</example>
	[StringLength(150, ErrorMessage = "Slug en fazla 150 karakter olabilir")]
	public string? Slug { get; set; }

	/// <summary>
	/// Kategori açıklaması
	/// </summary>
	/// <example>Elektronik ürünler kategorisi</example>
	[StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
	public string? Description { get; set; }

	/// <summary>
	/// Kategori resim URL'si
	/// </summary>
	/// <example>https://example.com/images/electronics.jpg</example>
	[StringLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir")]
	[Url(ErrorMessage = "Geçerli bir URL giriniz")]
	public string? ImageUrl { get; set; }

	/// <summary>
	/// Bootstrap icon sınıfı
	/// </summary>
	/// <example>bi-phone</example>
	[StringLength(50, ErrorMessage = "İkon sınıfı en fazla 50 karakter olabilir")]
	public string? IconClass { get; set; }

	/// <summary>
	/// Kategori aktif mi?
	/// </summary>
	/// <example>true</example>
	public bool IsActive { get; set; }

	/// <summary>
	/// Ana sayfada gösterilecek mi?
	/// </summary>
	/// <example>false</example>
	public bool IsFeatured { get; set; }

	/// <summary>
	/// Görüntüleme sırası
	/// </summary>
	/// <example>1</example>
	[Range(0, 999, ErrorMessage = "Görüntüleme sırası 0-999 arasında olmalıdır")]
	public int DisplayOrder { get; set; }

	/// <summary>
	/// SEO meta başlık
	/// </summary>
	/// <example>Elektronik Ürünler - Online Mağaza</example>
	[StringLength(60, ErrorMessage = "Meta başlık en fazla 60 karakter olabilir")]
	public string? MetaTitle { get; set; }

	/// <summary>
	/// SEO meta açıklama
	/// </summary>
	/// <example>En yeni elektronik ürünler, telefon, bilgisayar ve daha fazlası</example>
	[StringLength(160, ErrorMessage = "Meta açıklama en fazla 160 karakter olabilir")]
	public string? MetaDescription { get; set; }
}

/// <summary>
/// Kategori arama isteği
/// </summary>
public sealed class CategorySearchRequest
{
	/// <summary>
	/// Arama terimi (kategori adı veya slug'da aranır)
	/// </summary>
	/// <example>elektronik</example>
	[StringLength(100, ErrorMessage = "Arama terimi en fazla 100 karakter olabilir")]
	public string? SearchTerm { get; set; }

	/// <summary>
	/// Üst kategori ID'si (filtreleme için)
	/// </summary>
	/// <example>1</example>
	[Range(1, long.MaxValue, ErrorMessage = "Geçersiz üst kategori ID'si")]
	public long? ParentId { get; set; }

	/// <summary>
	/// Sadece aktif kategorileri getir
	/// </summary>
	/// <example>true</example>
	public bool? IsActive { get; set; }

	/// <summary>
	/// Sadece öne çıkan kategorileri getir
	/// </summary>
	/// <example>false</example>
	public bool? IsFeatured { get; set; }

	/// <summary>
	/// Sıralama alanı
	/// </summary>
	/// <example>DisplayOrder</example>
	[RegularExpression("^(Name|DisplayOrder|CreatedAt)$", ErrorMessage = "Geçersiz sıralama alanı")]
	public string? SortBy { get; set; } = "DisplayOrder";

	/// <summary>
	/// Sıralama yönü
	/// </summary>
	/// <example>Asc</example>
	[RegularExpression("^(Asc|Desc)$", ErrorMessage = "Geçersiz sıralama yönü")]
	public string? SortOrder { get; set; } = "Asc";

	/// <summary>
	/// Sayfa numarası (1'den başlar)
	/// </summary>
	/// <example>1</example>
	[Range(1, 1000, ErrorMessage = "Sayfa numarası 1-1000 arasında olmalıdır")]
	public int Page { get; set; } = 1;

	/// <summary>
	/// Sayfa boyutu (1-200 arası)
	/// </summary>
	/// <example>50</example>
	[Range(1, 200, ErrorMessage = "Sayfa boyutu 1-200 arasında olmalıdır")]
	public int PageSize { get; set; } = 50;
}

/// <summary>
/// Kategori arama sonucu
/// </summary>
public sealed class CategorySearchResponse
{
	/// <summary>
	/// Bulunan kategoriler listesi
	/// </summary>
	public List<CategoryDto> Categories { get; set; } = new();

	/// <summary>
	/// Toplam kategori sayısı
	/// </summary>
	/// <example>150</example>
	public int TotalCount { get; set; }

	/// <summary>
	/// Mevcut sayfa numarası
	/// </summary>
	/// <example>1</example>
	public int Page { get; set; }

	/// <summary>
	/// Sayfa boyutu
	/// </summary>
	/// <example>50</example>
	public int PageSize { get; set; }

	/// <summary>
	/// Toplam sayfa sayısı
	/// </summary>
	/// <example>3</example>
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

	/// <summary>
	/// Sonraki sayfa var mı?
	/// </summary>
	/// <example>true</example>
	public bool HasNextPage => Page < TotalPages;

	/// <summary>
	/// Önceki sayfa var mı?
	/// </summary>
	/// <example>false</example>
	public bool HasPreviousPage => Page > 1;
}
