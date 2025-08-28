# 🚀 Web Projesi TODO - Seller & Customer Portal Entegrasyonu

## 📊 Proje Durumu Analizi

### ✅ **Mevcut Durum**
- [x] Temel Web projesi yapısı (Razor Pages)
- [x] Bootstrap 5 entegrasyonu
- [x] Temel sayfa yapısı (Index, Products, Cart, StoreApplications)
- [x] API Client servisi
- [x] Anti-forgery koruması
- [x] Responsive tasarım

### 🔎 **Mevcut Durum (Kod Analizi)**
- Program.cs
  - [x] `AddRazorPages`, `AddHttpClient<ApiClient>` tanımlı
  - [x] Anti-forgery yapılandırması mevcut
  - [ ] Authentication/Authorization middleware kayıtlı değil (cookie/JWT yok)
- Sayfa Yapısı (`src/Web/Pages`)
  - [x] `Products/`, `Cart/`, `StoreApplications/Apply` mevcut
  - [ ] `Satici/` (Seller Portal) klasörü yok
  - [ ] `Hesabim/` (Customer Portal) klasörü yok
- Layout & Navigasyon (`Pages/Shared/_Layout.cshtml`)
  - [x] Modern header, arama çubuğu, mağaza başvurusu butonu
  - [ ] Role-aware menü/bağlantılar yok (Seller/Customer ayrımı yapılmıyor)
- URL/Route
  - [ ] Türkçe URL alias’ları tanımlı değil (varsayılan Razor Pages rotaları kullanılıyor)
- Yetkilendirme
  - [ ] `[Authorize]` kullanımı yok; klasör bazlı koruma tanımlı değil
- Frontend Assets
  - [x] `wwwroot/js/store-application.js` mevcut ve aktif kullanılıyor

### 🚧 **Yapılacaklar**
- [ ] Authentication & Authorization sistemi
- [ ] Seller Portal (Mağaza Yönetimi)
- [ ] Customer Portal (Müşteri Hesabı)
- [ ] Role-based navigation
- [ ] Data isolation (Seller/Customer veri ayrımı)
- [ ] Türkçe URL yapısı

### 📌 **Durum Özeti (Yapılmış / Eksik)**
- **Yapılmış**: UI iskeleti, Anti-forgery, ApiClient, Products/Cart/StoreApplications sayfaları
- **Eksik**: AuthN/AuthZ, Seller & Customer portal klasörleri, TR URL’ler, role-aware navigasyon, data isolation guard’ları

---

## 🎯 **SPRINT 1: Authentication & Authorization Sistemi (1 hafta)**

### 1.1 **Authentication Middleware Kurulumu**
- [ ] Cookie Authentication ekleme
- [ ] Role-based authorization policies
- [ ] Login/Register sayfaları
- [ ] Password reset functionality

### 1.2 **Program.cs Güncellemeleri**
```csharp
// Eklenecek servisler
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/giris";
        options.AccessDeniedPath = "/erisim-yasak";
        options.LogoutPath = "/cikis";
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});
```

### 1.3 **Login Sistemi**
- [ ] `/giris` sayfası (tek form, Customer/Seller otomatik tespit)
- [ ] `/kayit` sayfası (Customer kayıt)
- [ ] `/sifre-sifirla` sayfası
- [ ] Role-based otomatik yönlendirme:
  - Seller → `/satici/panel`
  - Customer → `/hesabim`

### 1.4 **Navigation Güncellemeleri**
- [ ] Header'da role-aware menü
- [ ] Giriş yapmamış: "Giriş Yap", "Kayıt Ol", "Mağaza Başvurusu"
- [ ] Customer girişli: "Hesabım", "Siparişlerim", "Çıkış"
- [ ] Seller girişli: "Satıcı Paneli", "Ürünlerim", "Siparişlerim", "Çıkış"

---

## 🏪 **SPRINT 2: Seller Portal - Temel Yapı (1 hafta)**

### 2.1 **Seller Portal Sayfa Yapısı**
```
/Pages/Satici/
├── Panel.cshtml (Dashboard)
├── Urunler/
│   ├── Index.cshtml (Ürün listesi)
│   ├── Yeni.cshtml (Ürün ekleme)
│   └── Duzenle.cshtml (Ürün düzenleme)
└── Shared/
    └── _SaticiLayout.cshtml (Seller-specific layout)
```

### 2.2 **Seller Dashboard (`/satici/panel`)**
- [ ] Mağaza özet bilgileri
- [ ] Son siparişler
- [ ] Satış istatistikleri
- [ ] Stok uyarıları
- [ ] Hızlı işlemler (ürün ekle, sipariş görüntüle)

### 2.3 **Ürün Yönetimi (`/satici/urunler`)**
- [ ] Ürün listesi (kendi store'undaki)
- [ ] Ürün ekleme formu
- [ ] Ürün düzenleme formu
- [ ] Ürün silme (soft delete)
- [ ] Toplu işlemler (aktif/pasif yapma)

### 2.4 **Authorization Guards**
- [ ] Tüm Seller sayfalarında `[Authorize(Roles="Seller")]`
- [ ] Store ownership validation
- [ ] Access denied sayfası (`/erisim-yasak`)

---

## 📦 **SPRINT 3: Seller Portal - Sipariş & Raporlar (1 hafta)**

### 3.1 **Sipariş Yönetimi (`/satici/siparisler`)**
- [ ] Sipariş listesi (kendi store'undaki)
- [ ] Sipariş detayları
- [ ] Sipariş durumu güncelleme
- [ ] Kargo takip numarası ekleme
- [ ] Sipariş filtreleme (tarih, durum, müşteri)

### 3.2 **Raporlar (`/satici/raporlar`)**
- [ ] Satış raporları (günlük, haftalık, aylık)
- [ ] Envanter raporları
- [ ] Müşteri analizi
- [ ] Komisyon raporları
- [ ] Export functionality (PDF, Excel)

### 3.3 **API Entegrasyonu**
- [ ] Seller-specific API endpoints
- [ ] Store ID otomatik ekleme (claim'den)
- [ ] Error handling ve loading states
- [ ] Real-time updates (SignalR)

---

## 👤 **SPRINT 4: Customer Portal & Türkçe URL'ler (1 hafta)**

### 4.1 **Customer Portal Sayfa Yapısı**
```
/Pages/Hesabim/
├── Index.cshtml (Profil bilgileri)
├── Siparisler/
│   ├── Index.cshtml (Sipariş listesi)
│   └── Detay.cshtml (Sipariş detayı)
└── Shared/
    └── _CustomerLayout.cshtml (Customer-specific layout)
```

### 4.2 **Customer Hesap Yönetimi**
- [ ] `/hesabim` - Profil bilgileri
- [ ] `/siparislerim` - Sipariş geçmişi
- [ ] `/adreslerim` - Adres yönetimi
- [ ] `/favorilerim` - Favori ürünler
- [ ] `/sifre-degistir` - Şifre değiştirme

### 4.3 **Türkçe URL Yapısı**
- [ ] `/urun/{slug}` - Ürün detayı
- [ ] `/kategori/{slug}` - Kategori sayfası
- [ ] `/sepet` - Alışveriş sepeti
- [ ] `/odeme` - Ödeme sayfası
- [ ] `/kargo-takip/{orderNo}` - Kargo takibi

### 4.4 **Navigation Güncellemeleri**
- [ ] Customer menüsü güncelleme
- [ ] Breadcrumb navigation
- [ ] Mobile-friendly navigation
- [ ] Search functionality

---

## 🔒 **SPRINT 5: Data Isolation & Security (1 hafta)**

### 5.1 **Data Isolation Implementation**
- [ ] Seller veri erişimi: Sadece kendi `storeId` kapsamındaki veriler
- [ ] Customer veri erişimi: Sadece yayınlanmış ve aktif mağaza ürünleri
- [ ] API level security: Store ID validation
- [ ] Database query optimization (storeId indexleri)

### 5.2 **Security Enhancements**
- [ ] CSRF protection (mevcut anti-forgery)
- [ ] XSS protection
- [ ] SQL injection prevention
- [ ] Rate limiting
- [ ] Input validation

### 5.3 **Error Handling**
- [ ] Global exception handling
- [ ] User-friendly error messages
- [ ] Logging ve monitoring
- [ ] 404, 403, 500 sayfaları

---

## 🧪 **SPRINT 6: Testing & Quality Assurance (1 hafta)**

### 6.1 **Unit Tests**
- [ ] Authentication service tests
- [ ] Authorization policy tests
- [ ] Page model tests
- [ ] Validation tests

### 6.2 **Integration Tests**
- [ ] Login flow tests
- [ ] Role-based access tests
- [ ] Data isolation tests
- [ ] API integration tests

### 6.3 **UI/UX Testing**
- [ ] Responsive design testing
- [ ] Cross-browser compatibility
- [ ] Accessibility testing
- [ ] Performance testing

---

## 🚀 **SPRINT 7: Performance & Optimization (1 hafta)**

### 7.1 **Performance Improvements**
- [ ] Page load optimization
- [ ] Image optimization
- [ ] CSS/JS minification
- [ ] Caching strategies

### 7.2 **SEO & Analytics**
- [ ] Meta tags optimization
- [ ] Structured data (JSON-LD)
- [ ] Google Analytics integration
- [ ] Sitemap generation

### 7.3 **Monitoring & Logging**
- [ ] Application performance monitoring
- [ ] Error tracking
- [ ] User behavior analytics
- [ ] Performance metrics

---

## 📋 **Teknik Gereksinimler**

### **Authentication & Authorization**
- Cookie-based authentication
- Role-based authorization (Seller, Customer)
- JWT token integration (API calls)
- Password hashing (bcrypt)
- Session management

### **Data Models**
```csharp
// User Claims
public class UserClaims
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } // "Seller" veya "Customer"
    public string? StoreId { get; set; } // Seller için
    public string FullName { get; set; }
}

// Seller Portal Models
public class SellerDashboardModel
{
    public StoreInfo Store { get; set; }
    public List<OrderSummary> RecentOrders { get; set; }
    public SalesStats SalesStats { get; set; }
    public InventoryAlerts InventoryAlerts { get; set; }
}

// Customer Portal Models
public class CustomerProfileModel
{
    public UserInfo User { get; set; }
    public List<Address> Addresses { get; set; }
    public List<OrderSummary> OrderHistory { get; set; }
}
```

### **API Integration**
- Seller API calls: Otomatik `storeId` ekleme
- Customer API calls: Public data only
- Error handling ve retry logic
- Loading states ve optimistic updates

---

## 🎯 **Başarı Kriterleri (Definition of Done)**

### **Sprint 1 - Authentication**
- [ ] `/giris` ile login çalışır
- [ ] Role-based otomatik yönlendirme çalışır
- [ ] Navigation role-aware çalışır
- [ ] Authorization policies aktif

### **Sprint 2 - Seller Portal Temel**
- [ ] `/satici/panel` erişilebilir
- [ ] Ürün listesi görüntülenir
- [ ] Ürün ekleme/düzenleme çalışır
- [ ] Seller-only access korunur

### **Sprint 3 - Seller Portal Gelişmiş**
- [ ] Sipariş yönetimi çalışır
- [ ] Raporlar görüntülenir
- [ ] API entegrasyonu tamamlanır
- [ ] Real-time updates çalışır

### **Sprint 4 - Customer Portal**
- [ ] `/hesabim` erişilebilir
- [ ] Sipariş geçmişi görüntülenir
- [ ] Türkçe URL'ler çalışır
- [ ] Customer navigation aktif

### **Sprint 5 - Security**
- [ ] Data isolation çalışır
- [ ] Security headers aktif
- [ ] Error handling tamamlanır
- [ ] Performance monitoring aktif

### **Sprint 6 - Testing**
- [ ] Unit test coverage > 70%
- [ ] Integration tests çalışır
- [ ] UI/UX testing tamamlanır
- [ ] Cross-browser compatibility

### **Sprint 7 - Optimization**
- [ ] Page load time < 3 saniye
- [ ] SEO optimization tamamlanır
- [ ] Analytics integration aktif
- [ ] Performance metrics izlenir

---

## 📅 **Zaman Çizelgesi**

### **Toplam Süre: 7 Hafta**
- **Sprint 1**: Authentication & Authorization (1 hafta)
- **Sprint 2**: Seller Portal Temel (1 hafta)
- **Sprint 3**: Seller Portal Gelişmiş (1 hafta)
- **Sprint 4**: Customer Portal & TR URL'ler (1 hafta)
- **Sprint 5**: Data Isolation & Security (1 hafta)
- **Sprint 6**: Testing & QA (1 hafta)
- **Sprint 7**: Performance & Optimization (1 hafta)

### **Milestone'lar**
- **Hafta 1**: Authentication sistemi çalışır
- **Hafta 3**: Seller portal tamamen fonksiyonel
- **Hafta 4**: Customer portal aktif
- **Hafta 5**: Security ve data isolation tamamlanır
- **Hafta 6**: Testing tamamlanır
- **Hafta 7**: Production-ready

---

## 🚨 **Risk Analizi**

### **Yüksek Risk**
- **Authentication complexity**: Role-based routing karmaşık olabilir
- **Data isolation**: Store ID validation kritik
- **Performance**: Çok sayfa yükleme yavaş olabilir

### **Orta Risk**
- **API integration**: Seller/Customer endpoint'leri farklı
- **UI/UX consistency**: İki portal arası tutarlılık
- **Testing coverage**: Kapsamlı test yazımı zaman alabilir

### **Düşük Risk**
- **Bootstrap integration**: Mevcut tasarım sistemi
- **Database schema**: Mevcut yapı uygun
- **Deployment**: Tek proje, tek deployment

---

## 💡 **Geliştirme Önerileri**

1. **Incremental Development**: Her sprint'te çalışan bir sistem
2. **Feature Flags**: Yeni özellikleri aşamalı aktif etme
3. **A/B Testing**: UI/UX iyileştirmeleri için
4. **Performance Monitoring**: Her sprint'te performance metrics
5. **User Feedback**: Seller ve Customer feedback'i toplama

---

*Son Güncelleme: 2024-12-28*
*Proje Durumu: %15 Tamamlandı*
*Aktif Sprint: Sprint 1 - Authentication & Authorization*
*Tahmini Tamamlanma: 7 Hafta*
*Toplam Sprint: 7*
