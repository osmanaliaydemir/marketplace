using Application.Abstractions;
using Application.Exceptions;
using Domain.Entities;

namespace Application.Services;

public sealed class BusinessRuleValidator
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ISellerRepository _sellerRepository;

    public BusinessRuleValidator(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IStoreRepository storeRepository,
        ISellerRepository sellerRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _storeRepository = storeRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task ValidateProductCreationAsync(long sellerId, long categoryId, long storeId, string name)
    {
        // Seller var mı?
        var seller = await _sellerRepository.GetByIdAsync(sellerId);
        if (seller == null)
        {
            throw new BusinessRuleViolationException(
                "SellerNotFound", 
                "Belirtilen satıcı bulunamadı", 
                new { SellerId = sellerId });
        }

        // Seller aktif mi?
        if (!seller.IsActive)
        {
            throw new BusinessRuleViolationException(
                "SellerNotActive", 
                "Satıcı hesabı aktif değil",
                new { SellerId = sellerId });
        }

        // Category var mı?
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category == null)
        {
            throw new BusinessRuleViolationException(
                "CategoryNotFound", 
                "Belirtilen kategori bulunamadı",
                new { CategoryId = categoryId });
        }

        // Category aktif mi?
        if (!category.IsActive)
        {
            throw new BusinessRuleViolationException(
                "CategoryNotActive", 
                "Belirtilen kategori aktif değil",
                new { CategoryId = categoryId });
        }

        // Store var mı?
        var store = await _storeRepository.GetByIdAsync(storeId);
        if (store == null)
        {
            throw new BusinessRuleViolationException(
                "StoreNotFound", 
                "Belirtilen mağaza bulunamadı",
                new { StoreId = storeId });
        }

        // Store aktif mi?
        if (!store.IsActive)
        {
            throw new BusinessRuleViolationException(
                "StoreNotActive", 
                "Belirtilen mağaza aktif değil",
                new { StoreId = storeId });
        }

        // Seller bu store'un sahibi mi?
        if (store.SellerId != sellerId)
        {
            throw new BusinessRuleViolationException(
                "StoreOwnershipViolation", 
                "Bu mağazada ürün oluşturma yetkiniz yok",
                new { SellerId = sellerId, StoreId = storeId });
        }

        // Aynı isimde ürün var mı?
        var existingProducts = await _productRepository.GetAsync(p => 
            p.Name.ToLower() == name.ToLower() && 
            p.StoreId == storeId);

        if (existingProducts.Any())
        {
            throw new BusinessRuleViolationException(
                "DuplicateProductName", 
                "Bu mağazada aynı isimde başka bir ürün zaten mevcut",
                new { ProductName = name, StoreId = storeId });
        }
    }

    public async Task ValidateProductUpdateAsync(long productId, long sellerId, long? newCategoryId = null)
    {
        // Product var mı?
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new BusinessRuleViolationException(
                "ProductNotFound", 
                "Belirtilen ürün bulunamadı",
                new { ProductId = productId });
        }

        // Seller bu ürünün sahibi mi?
        if (product.SellerId != sellerId)
        {
            throw new BusinessRuleViolationException(
                "ProductOwnershipViolation", 
                "Bu ürünü güncelleme yetkiniz yok",
                new { ProductId = productId, SellerId = sellerId });
        }

        // Yeni kategori belirtilmişse, kategori kontrolü yap
        if (newCategoryId.HasValue && newCategoryId.Value != product.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(newCategoryId.Value);
            if (category == null)
            {
                throw new BusinessRuleViolationException(
                    "CategoryNotFound", 
                    "Belirtilen kategori bulunamadı",
                    new { CategoryId = newCategoryId.Value });
            }

            if (!category.IsActive)
            {
                throw new BusinessRuleViolationException(
                    "CategoryNotActive", 
                    "Belirtilen kategori aktif değil",
                    new { CategoryId = newCategoryId.Value });
            }
        }
    }

    public async Task ValidateProductDeletionAsync(long productId, long sellerId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new BusinessRuleViolationException(
                "ProductNotFound", 
                "Belirtilen ürün bulunamadı",
                new { ProductId = productId });
        }

        if (product.SellerId != sellerId)
        {
            throw new BusinessRuleViolationException(
                "ProductOwnershipViolation", 
                "Bu ürünü silme yetkiniz yok",
                new { ProductId = productId, SellerId = sellerId });
        }

        // TODO: Aktif siparişlerde olan ürünler silinemez kontrolü
        // TODO: Sepetlerde olan ürünler kontrolü
    }
}
