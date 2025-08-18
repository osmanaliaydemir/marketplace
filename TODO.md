## TODO - Marketplace (TR, Multi-Vendor)

- [ ] API (Controllers, DTO, Validasyon)
  - [ ] Customer: `ProductsController`, `CategoriesController`, `SearchController`
  - [ ] Sepet: `CartController` (add/get/delete), çok satıcılı sepet
  - [ ] Checkout: `CheckoutController` (session), `PaymentsController` (callback), `WebhooksController` (paytr)
  - [ ] Orders: `OrdersController` (müşteri), `SellerOrdersController` (satıcı)
  - [ ] Admin: `AdminSellersController` (onay/komisyon), `AdminRefundsController`, `ReportsController`
  - [ ] DTO modelleri ve FluentValidation kuralları
  - [ ] RFC7807: `ProblemDetailsMiddleware` -> correlation-id, error codes
  - [ ] Swagger: tüm endpoint’lere örnek istek/yanıt, security scheme


- [ ] Ödeme Entegrasyonu - PayTR Marketplace
  - [ ] `PaytrAdapter`: iFrame/Hosted init (token), HMAC doğrulama, refund, rapor çekme
  - [ ] Konfigürasyon: `merchant_id`, `merchant_key`, `merchant_salt`, endpoint URL’leri (sandbox/production)
  - [ ] `merchant_oid` üretimi ve sipariş/ödeme korelasyonu
  - [ ] Idempotency: Redis SETNX + TTL (merchant_oid, webhook event id)
  - [ ] Sandbox E2E: iFrame -> callback -> OK

- [ ] Uygulama Servisleri
  - [ ] `CheckoutService`: sepet -> sipariş -> `order_groups` -> PayTR init
  - [ ] `OrderService`: sipariş/fulfillment/kargo güncellemeleri
  - [ ] `PaymentService`: callback işleme, status geçişleri, refunds
  - [ ] `LedgerService`: çift kayıt (Dr/Cr), yuvarlama/komisyon kuralları
  - [ ] `payment_splits`: satıcı payı + platform komisyonu hesaplama kuralları


- [ ] Güvenlik
  - [ ] JWT rol tabanlı yetki: `Customer`, `Seller`, `Admin` ve policy’ler
  - [ ] CORS allowlist (Web Razor, Flutter mobil origin’leri)
  - [ ] Rate Limiting profilleri (public, auth, webhook)
  - [ ] Loglarda PII maskeleme (KVKK)

- [ ] Ölçülebilirlik & Gözlemlenebilirlik
  - [ ] Serilog yapılandırması (correlation-id, request logging)
  - [ ] HealthChecks: SQL, Redis, (ops.) PayTR DNS/HTTP reachability
  - [ ] Temel metrikler (istek sayısı/süreleri, hata oranı)

- [ ] Cache & Idempotency (Redis)
  - [ ] Ürün/kategori listeleme ve arama için caching (TTL)
  - [ ] Callback idempotency anahtarları (merchant_oid/event)

- [ ] Arka Plan İşleri
  - [ ] Outbox işlemcisi: domain event -> outbox -> publish/işle
  - [ ] `PaytrReconciliationWorker`: günlük rapor çekme, payments/ledger eşleme, fark raporu
  - [ ] Zamanlama ve retry/backoff stratejileri

- [ ] Testler
  - [ ] Unit: `CheckoutService`, `PaymentService`, `LedgerService` için senaryolar
  - [ ] Integration: PayTR callback HMAC + idempotency, ledger Dr/Cr tutarlılığı
  - [ ] Refund senaryoları (tam/kısmi) ve ters kayıtlar
  - [ ] Smoke: temel endpoint’ler 200/401/403 davranışları

- [ ] DevOps
  - [ ] Docker Compose: API + SQL Server + Redis (gerekirse seed)
  - [ ] CI: restore/build/test; migration çalıştırma adımı
  - [ ] Secrets yönetimi (User Secrets / ortam değişkenleri)

- [ ] Dokümantasyon
  - [ ] README: kurulum, migrate, geliştirme akışı, .env örnekleri
  - [ ] Swagger açıklamaları: iFrame paramları, webhook örnek payload’ı
  - [ ] İstemci entegrasyon notları (Razor, Flutter)

- [ ] Başarı Ölçütleri (DoD)
  - [ ] iFrame ödeme -> callback "OK" -> `orders/payments/ledger` tutarlı
  - [ ] Satıcı paneli: sipariş/kargo/rapor ekranları çalışır
  - [ ] Admin: satıcı onay/komisyon/iadeler yönetilebilir
  - [ ] Swagger tam ve güncel
