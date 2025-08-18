# Pazaryeri (Marketplace) — .NET 9 Web API (Controllers)

Kapsam: Çoklu satıcı pazaryeri. Ödeme PayTR iFrame/Hosted Checkout ile (Marketplace/Split destekli), kart verisi sistemde tutulmaz (SAQ‑A). Üç rol: **Customer**, **Seller**, **Admin**. Web: Razor + HTML/JS/CSS. Mobil: Flutter (ayrı ekip).

---

## 1) Mimari Genel Bakış

- **Dil/Çatı:** .NET 9 + ASP.NET Core **Controllers** (minimal değil)
- **Veritabanı:** SQL Server 2022 (+ Dapper), Redis (idempotency, cache)
- **İletişim:** HTTP REST + Webhook (PayTR Bildirim URL), opsiyonel RabbitMQ (v2)
- **Katmanlama:** Modüler monolit, feature-first (vertical slice) paketleme; fakat Controller tabanlı API
- **Gözlemlenebilirlik:** Serilog + OpenTelemetry (OTLP), HealthChecks
- **Doğrulama/Mapping:** FluentValidation, Mapster
- **Versiyonlama:** URL tabanlı `/api/v1/...`

```
src/
  Api/                       # ASP.NET Core (Controllers)
    Controllers/
      Customer/*.cs
      Seller/*.cs
      Admin/*.cs
      Payments/PaytrController.cs
    Filters/
    Middlewares/
    Config/
  Application/               # Use-case/Service/Handlers
  Domain/                    # Entity/ValueObject/Events
  Infrastructure/            # Dapper repos, Redis, PayTR adapter, Email,SMS
  Tests/
```

---

## 2) Güvenlik ve Uyumluluk

- **Auth:** JWT Bearer. Claims: `sub`, `role` ∈ {`Customer`,`Seller`,`Admin`}, `seller_id`
- **Policy:** `[Authorize(Roles="Seller")]` vb. + `RequireClaim("seller_id")`
- **CORS:** Web & Mobil origin’leri allowlist
- **Rate Limiting:** .NET RateLimiter (sliding window) — checkout ve webhook’larda sıkı
- **Idempotency:** `Idempotency-Key` header → Redis+DB kayıt; tekrar çağrılarda 409/200 aynı cevap
- **Hata Modeli:** RFC7807 `ProblemDetails` (tek format)
- **Log/PII:** Kişisel veriyi maskeler; KVKK saklama politikası
- **PCI/Kart:** Kart verisi sisteme **girmez** (SAQ‑A). 3DS2 akışları PSP’de

---

## 3) Veri Modeli (çekirdek)

**Not:** Parasal alanlar `DECIMAL(19,4)`, tarih `DATETIME2(3) UTC`.

**Hesap/Satıcı**

- `sellers(id BIGINT PK, user_id BIGINT, paytr_submerchant_id NVARCHAR(64), kyc_status TINYINT, commission_rate DECIMAL(5,2), iban NVARCHAR(34), created_at, modified_at, is_active BIT)`
- `stores(id BIGINT PK, seller_id BIGINT FK, name, slug, logo_url, is_active, created_at, modified_at)`

**Katalog**

- `categories(id, parent_id NULL, name, slug, is_active)`
- `products(id, seller_id, category_id, name, slug, description, price, currency, is_active, created_at, modified_at)`
- `product_variants(id, product_id, sku, price, stock_qty, is_active)`
- `inventories(id, variant_id, on_hand, reserved, updated_at)`

**Sipariş/Ödeme**

- `orders(id, buyer_id NULL, email, phone, ship_address_json, total, currency, status, created_at)`
- `order_groups(id, order_id, seller_id, items_total, shipping_total, group_total, status)`
- `order_items(id, order_group_id, product_id, variant_id NULL, name, qty, unit_price, total)`
- `payments(id, order_id, provider NVARCHAR(32), provider_tx NVARCHAR(128), status, gross, fee_platform, fee_psp, net_to_sellers, created_at, captured_at NULL)`
- `refunds(id, payment_id, order_group_id NULL, amount, reason, status, created_at)`

**Ledger (çift kayıt)**

- `ledger_transactions(id, ref_type, ref_id, ts)`
- `ledger_postings(id, transaction_id, account, debit, credit)`
  - Hesaplar: `CASH`, `PLATFORM_FEES`, `PAYTR_FEES`, `SELLER_PAYABLE`

**Webhook**

- `webhook_deliveries(id, event_type, payload, signature, status, attempts, created_at, delivered_at NULL)`

**Read Modelleri (denormalize)**

- `read.product_search(id, seller_id, name, category_path, price, rating, searchable_text)`
- `read.seller_dashboard_daily(seller_id, day, orders, gross, refunds, net)`

**DB Ayarları**

```sql
ALTER DATABASE [Marketplace] SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE;
ALTER DATABASE [Marketplace] SET TARGET_RECOVERY_TIME = 60 SECONDS;
```

**Temel İndeks Önerileri**

- `orders (buyer_id, created_at DESC) INCLUDE(total,status)`
- `order_groups (order_id, seller_id)`
- `payments (order_id), payments(provider_tx)`
- `products (seller_id, is_active, price)` + Full-Text(`name`,`description`)

---

## 4) API Yüzeyi (V1, özet)

**Customer**

- `GET /api/v1/customer/products` (list/filter/search)
- `GET /api/v1/customer/products/{id}`
- `GET /api/v1/customer/categories`
- `POST /api/v1/customer/cart` (add/update/remove)
- `POST /api/v1/customer/checkout/session` → PayTR token
- `GET /api/v1/customer/orders` (auth)
- `GET /api/v1/customer/orders/{id}`

**Seller** (Authorize: Seller)

- `GET /api/v1/seller/me` (profil + KYC durumu)
- `POST /api/v1/seller/products` | `PUT` | `DELETE`
- `GET /api/v1/seller/orders` | `GET /{id}` | `PUT /{id}/status`
- `POST /api/v1/seller/shipping/labels` (entegrasyon/manuel)
- `GET /api/v1/seller/reports/daily`

**Admin** (Authorize: Admin)

- `GET/POST /api/v1/admin/sellers` (onay, komisyon oranları)
- `GET /api/v1/admin/orders`
- `GET /api/v1/admin/payments`
- `POST /api/v1/admin/refunds/{paymentId}` (kısmi/tam)
- `GET /api/v1/admin/reports/finance`

**Payments (PSP Adapter)**

- `POST /api/v1/payments/paytr/token` (checkout session)
- `POST /api/v1/payments/paytr/callback` (Bildirim URL — server‑to‑server)

**Standartlar**

- **Error:** `ProblemDetails`
- **Idempotency:** `Idempotency-Key`
- **Version:** `/api/v1/...`

---

## 5) PayTR Entegrasyonu (kritik akış)

1. **Token**: Server tarafında `merchant_oid`, `payment_amount`, `user_basket(base64)` ve HMAC ile `paytr_token` oluştur; PayTR’den `token` al; iFrame/redirect aç.
2. **Callback (Bildirim URL)**: PayTR, ödeme sonucunu POST eder → imzayı doğrula → `payments` & `orders/order_groups` durumlarını **idempotent** finalize et → "OK" döndür.
3. **Split/Marketplace**: Satıcı bazlı komisyon oranı (platform) ve net bedel PayTR’de yönetilir; sistemde ledger kayıtları oluşturulur.
4. **Durum Sorgu & İade**: Yönetim/Satıcı panelinden kısmi/tam iade → PSP API → ledger senkronizasyonu.

---

## 6) Konfig (appsettings.json ör.)

```json
{
  "ConnectionStrings": { "Default": "Server=...;Database=Marketplace;..." },
  "Jwt": { "Authority": "https://auth.example", "Audience": "marketplace-api", "Key": "<dev-only>" },
  "Paytr": {
    "MerchantId": "",
    "MerchantKey": "",
    "MerchantSalt": "",
    "CallbackUrl": "https://api.example.com/api/v1/payments/paytr/callback",
    "OkUrl": "https://web.example.com/payment/success",
    "FailUrl": "https://web.example.com/payment/fail"
  }
}
```

---

## 7) Program.cs (özet iskelet)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "Marketplace API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new() { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT" });
    c.AddSecurityRequirement(new() { { new() { Reference = new() { Id = "Bearer", Type = ReferenceType.SecurityScheme } }, new string[]{} } });
});

builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddAuthorization();

// Dapper DB
builder.Services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(builder.Configuration.GetConnectionString("Default")));
// Redis, RateLimiter, Serilog, Validation, Mapster ...

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## 8) Örnek Controller’lar (kısa)

**ProductsController (Customer)**

```csharp
[ApiController]
[Route("api/v1/customer/products")]
public sealed class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ProductQuery q, [FromServices] IProductQueries svc)
        => Ok(await svc.SearchAsync(q));
}
```

**PaytrController (Payments)**

```csharp
[ApiController]
[Route("api/v1/payments/paytr")]
public sealed class PaytrController : ControllerBase
{
    [HttpPost("token")]
    public async Task<IActionResult> CreateToken([FromBody] CheckoutRequest req, [FromServices] IPaytrService paytr)
        => Ok(await paytr.CreateTokenAsync(req));

    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback()
    {
        var raw = await new StreamReader(Request.Body).ReadToEndAsync();
        var ok = await _svc.HandleCallbackAsync(Request.Headers, raw);
        return ok ? Content("OK") : Unauthorized();
    }
}
```

---

## 9) Test & Kalite

- **Unit/Integration**: Controller + Repo testleri; PayTR callback için imza doğrulama testi
- **Contract Test**: Swagger/OpenAPI → Schemathesis
- **Static Analysis**: Roslyn analyzers, nullable enabled, StyleCop

---

## 10) Sprint Planı (MVP → V1)

**Sprint 0 — Altyapı** (1 hafta)

- Proje iskeleti (Controllers, Swagger, Auth stub)
- MSSQL şema v1 (migrations), RCSI, seed
- Redis, Serilog, HealthChecks, ProblemDetails
- CI/CD: build, test, docker image, dev deploy **Kabul:** `/swagger` açık, health ok, temel tablolara insert testi

**Sprint 1 — Customer + Checkout (PayTR Token)** (1–2 hafta)

- Catalog: Product/Category list/search (read model opsiyonel)
- Cart + Checkout session → `POST /payments/paytr/token`
- Order draft + order\_groups oluşturma (txn)
- Idempotency-Key altyapısı **Kabul:** Demo akışında iFrame açılıyor, order draft oluşuyor

**Sprint 2 — PayTR Callback + Sipariş Finalizasyonu** (1 hafta)

- `POST /payments/paytr/callback` (imza, idempotent)
- Payments → paid, orders/order\_groups → paid
- Ledger (CASH, PLATFORM\_FEES, SELLER\_PAYABLE) kayıtları
- Admin: payments list (readonly) **Kabul:** Sandbox’tan başarılı ödeme → sistemde paid ve ledger var

**Sprint 3 — Seller Panel API’ları** (1–2 hafta)

- Seller auth/claims, ürün CRUD, stok
- Seller orders list + status update, kargo kodu
- Seller raporları (daily summary) **Kabul:** Satıcı ürün ekleyip sipariş görebiliyor

**Sprint 4 — Admin & İade/Mutabakat** (1–2 hafta)

- Admin: seller onboarding (onay/komisyon), global raporlar
- Refund (tam/kısmi) + ledger yansımaları
- Reconciliation job (PayTR raporu ↔ DB) **Kabul:** Kısmi iade akışı ve günlük mutabakat raporu çalışır

**Backlog (v1.1+)**: Kargo entegrasyonları, çoklu PSP adapter, arama için Full‑Text/Elastic, kupon/indirim, fatura entegrasyonu, çoklu para birimi.

---

## 11) Tanımlar (DoD)

- Swagger şemasında endpoint var, otomasyon testleri geçiyor, loglar PII’siz, dokümantasyon güncel (README + Postman koleksiyonu), hata modelinde `ProblemDetails`, güvenlik ve idempotency kontrolleri mevcut.

Bu doküman sprint’ler boyunca versiyonlanacak; şema ve endpoint’ler değişirse migration ve OpenAPI revizyonu birlikte ilerletilecek.

