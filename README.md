# Marketplace - Multi-Vendor E-Ticaret Platformu

Türkiye odaklı, çok satıcılı e-ticaret platformu. .NET 9, SQL Server ve modern web teknolojileri ile geliştirilmiştir.

## Özellikler

### Kimlik Doğrulama & Yetkilendirme
- JWT tabanlı authentication
- Role-based authorization (Admin, Seller, Customer)
- Session management
- Secure password hashing

### Mağaza Yönetimi
- Mağaza başvuru sistemi
- Admin onay süreci
- Mağaza profil yönetimi
- Kategori ve ürün yönetimi

### Ödeme Sistemi
- PayTR Marketplace entegrasyonu
- Otomatik komisyon dağıtımı
- Çoklu satıcı ödeme işlemi
- Refund ve iptal yönetimi

### Dashboard
- Admin dashboard
- Satış istatistikleri
- Mağaza başvuru yönetimi
- Kullanıcı yönetimi

##  Teknolojiler

### Backend
- **.NET 9** - Web API
- **SQL Server 2022** - Ana veritabanı
- **Dapper** - Micro ORM
- **Redis** - Cache ve idempotency
- **Serilog** - Structured logging

### Frontend
- **ASP.NET Core Razor Pages** - Web UI
- **Bootstrap 5** - UI Framework
- **Bootstrap Icons** - Icon library

### Mimari
- **Domain-Driven Design (DDD)**
- **Clean Architecture**
- **Repository Pattern**
- **Unit of Work Pattern**
- **CQRS Pattern**

## Proje Yapısı

```
src/
├── Api/                    # Web API projesi
├── Application/            # Application layer
├── Domain/                 # Domain entities ve business logic
├── Infrastructure/         # Data access ve external services
├── Web/                    # Müşteri web arayüzü
├── Dashboard/              # Admin dashboard
└── BackgroundWorkers/      # Background job'lar

tests/                      # Unit ve integration testler
```

## Kurulum

### Gereksinimler
- .NET 9 SDK
- SQL Server 2022
- Redis (opsiyonel)

### 1. Repository'yi klonla
```bash
git clone https://github.com/osmanaliaydemir/marketplace.git
cd marketplace
```

### 2. Veritabanını kur
```sql
-- SQL Server'da Marketplace database'i oluştur
CREATE DATABASE Marketplace;
GO

-- sql.md dosyasındaki script'leri çalıştır
```

### 3. Test kullanıcılarını ekle
```sql
-- insert_test_users.sql dosyasındaki script'i çalıştır
-- Şifre: admin123
```

### 4. Projeyi çalıştır
```bash
# Solution'ı build et
dotnet build

# API'yi çalıştır
cd src/Api
dotnet run

# Dashboard'ı çalıştır (yeni terminal)
cd src/Dashboard
dotnet run
```

## Test Kullanıcıları

| Email | Şifre | Rol |
|-------|-------|-----|
| admin@marketplace.local | admin123 | Admin |
| seller@marketplace.local | admin123 | Seller |
| customer@marketplace.local | admin123 | Customer |

## API Dokümantasyonu

- **Swagger UI**: `https://localhost:7001/swagger`
- **Health Check**: `https://localhost:7001/health`

## 🧪 Test

```bash
# Tüm testleri çalıştır
dotnet test

# Belirli proje testlerini çalıştır
dotnet test tests/Infrastructure.Tests/
```

## Katkıda Bulunma

1. Fork yap
2. Feature branch oluştur (`git checkout -b feature/amazing-feature`)
3. Commit yap (`git commit -m 'Add amazing feature'`)
4. Push yap (`git push origin feature/amazing-feature`)
5. Pull Request oluştur

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## Geliştirici

**Osman Ali Aydemir**
- .NET Developer & Software Architect
- Fullstack Developer (Backend, Frontend, Database)
- [GitHub](https://github.com/osmanaliaydemir)

## Destek

Sorunlar için [GitHub Issues](https://github.com/osmanaliaydemir/marketplace/issues) kullanın.
