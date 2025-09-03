# 🚀 Marketplace API Documentation

## 📋 Genel Bakış

Marketplace API, e-ticaret platformu için kapsamlı bir REST API'si sağlar. Bu API ürün yönetimi, sipariş işlemleri, ödeme entegrasyonu, kullanıcı yönetimi ve mağaza yönetimi işlevlerini destekler.

## 🔗 Base URL

```
Development: https://localhost:7001
Production: https://api.marketplace.com
```

## 🔐 Kimlik Doğrulama

API iki farklı kimlik doğrulama yöntemi destekler:

### JWT Bearer Token
```
Authorization: Bearer <your-jwt-token>
```

### API Key (Entegrasyonlar için)
```
X-API-Key: <your-api-key>
```

## 📚 Endpoint Kategorileri

### 1. 🔐 Kimlik Doğrulama (Authentication)
- `POST /api/auth/login` - Kullanıcı girişi
- `POST /api/auth/register` - Yeni kullanıcı kaydı
- `POST /api/auth/refresh` - Token yenileme
- `POST /api/auth/logout` - Kullanıcı çıkışı

### 2. 📦 Ürün Yönetimi (Products)
- `GET /api/products` - Ürün listesi (filtrelenebilir)
- `GET /api/products/{id}` - Ürün detayı
- `GET /api/products/by-slug/{slug}` - Slug ile ürün detayı
- `POST /api/products` - Yeni ürün oluşturma (Satıcı)
- `PUT /api/products/{id}` - Ürün güncelleme (Satıcı)
- `POST /api/products/{id}/publish` - Ürün yayınlama (Satıcı)
- `POST /api/products/{id}/unpublish` - Ürün yayından kaldırma (Satıcı)

### 3. 🛒 Sepet Yönetimi (Cart)
- `GET /api/cart` - Sepet içeriği
- `POST /api/cart/items` - Sepete ürün ekleme
- `PUT /api/cart/items/{id}` - Sepetteki ürün güncelleme
- `DELETE /api/cart/items/{id}` - Sepetten ürün silme
- `DELETE /api/cart` - Sepeti temizleme

### 4. 🛍️ Sipariş Yönetimi (Orders)
- `GET /api/orders` - Sipariş listesi
- `GET /api/orders/{id}` - Sipariş detayı
- `POST /api/orders` - Yeni sipariş oluşturma
- `PUT /api/orders/{id}/cancel` - Sipariş iptal etme
- `PUT /api/orders/{id}/status` - Sipariş durumu güncelleme

### 5. 💳 Ödeme İşlemleri (Payments)
- `POST /api/payments/process` - Ödeme işlemi
- `GET /api/payments/{id}` - Ödeme detayı
- `POST /api/payments/refund` - İade işlemi
- `GET /api/payments/history` - Ödeme geçmişi

### 6. 🏪 Mağaza Yönetimi (Stores)
- `GET /api/stores` - Mağaza listesi
- `GET /api/stores/{id}` - Mağaza detayı
- `POST /api/stores` - Yeni mağaza oluşturma
- `PUT /api/stores/{id}` - Mağaza güncelleme

### 7. 👥 Kullanıcı Yönetimi (Users)
- `GET /api/users/profile` - Kullanıcı profili
- `PUT /api/users/profile` - Profil güncelleme
- `POST /api/users/change-password` - Şifre değiştirme
- `GET /api/users/addresses` - Adres listesi
- `POST /api/users/addresses` - Yeni adres ekleme

## 📊 Response Formatları

### Başarılı Yanıt
```json
{
  "data": {
    // Response data
  },
  "message": "İşlem başarılı",
  "success": true
}
```

### Hata Yanıtı
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Geçersiz veri",
    "details": [
      {
        "field": "email",
        "message": "Geçerli bir email adresi giriniz"
      }
    ]
  },
  "success": false
}
```

## 📝 Örnek Kullanımlar

### Ürün Listesi Alma
```bash
curl -X GET "https://api.marketplace.com/api/products?searchTerm=iphone&categoryId=1&page=1&pageSize=10" \
  -H "Accept: application/json"
```

### Yeni Ürün Oluşturma
```bash
curl -X POST "https://api.marketplace.com/api/products" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone 15 Pro",
    "description": "Apple iPhone 15 Pro",
    "price": 29999.99,
    "categoryId": 1,
    "storeId": 1
  }'
```

### Sipariş Oluşturma
```bash
curl -X POST "https://api.marketplace.com/api/orders" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "items": [
      {
        "productId": 1,
        "quantity": 2
      }
    ],
    "shippingAddressId": 1,
    "billingAddressId": 1
  }'
```

### Kullanıcı Çıkış Yapma
```bash
curl -X POST "https://api.marketplace.com/api/auth/logout" \
  -H "Authorization: Bearer <your-jwt-token>"
```

## 🔧 Rate Limiting

API rate limiting uygular:
- Genel endpoint'ler: 1000 istek/saat
- Kimlik doğrulama: 100 istek/saat
- Ödeme işlemleri: 50 istek/saat

## 📋 HTTP Status Codes

| Code | Açıklama |
|------|----------|
| 200 | Başarılı |
| 201 | Oluşturuldu |
| 400 | Geçersiz İstek |
| 401 | Yetkilendirme Hatası |
| 403 | Erişim Reddedildi |
| 404 | Bulunamadı |
| 422 | Doğrulama Hatası |
| 429 | Çok Fazla İstek |
| 500 | Sunucu Hatası |

## 🛠️ Geliştirme

### Swagger UI
Geliştirme ortamında Swagger UI'a erişim:
```
https://localhost:7001
```

### Postman Collection
Postman collection'ı indir: [Marketplace API.postman_collection.json](https://github.com/marketplace/api/postman-collection.json)

## 📞 Destek

- **Email**: api-support@marketplace.com
- **Dokümantasyon**: https://docs.marketplace.com/api
- **GitHub Issues**: https://github.com/marketplace/api/issues

## 📄 Lisans

Bu API MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakınız.
