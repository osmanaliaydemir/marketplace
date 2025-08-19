-- Marketplace Test Data Insertion Script
-- Bu script'i SQL Server Management Studio'da çalıştırın

USE marketplace;
GO

-- 1. Test Users (Eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM app_users WHERE email = 'admin@marketplace.local')
BEGIN
    INSERT INTO app_users (email, password_hash, full_name, role, is_active, created_at)
    VALUES ('admin@marketplace.local', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Admin User', 'Admin', 1, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM app_users WHERE email = 'seller@marketplace.local')
BEGIN
    INSERT INTO app_users (email, password_hash, full_name, role, is_active, created_at)
    VALUES ('seller@marketplace.local', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Seller User', 'Seller', 1, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM app_users WHERE email = 'customer@marketplace.local')
BEGIN
    INSERT INTO app_users (email, password_hash, full_name, role, is_active, created_at)
    VALUES ('customer@marketplace.local', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Customer User', 'Customer', 1, GETUTCDATE());
END

-- 2. Test Categories
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'elektronik')
BEGIN
    INSERT INTO categories (name, slug, description, is_active, is_featured, display_order, created_at)
    VALUES 
    ('Elektronik', 'elektronik', 'Elektronik ürünler ve aksesuarlar', 1, 1, 1, GETUTCDATE()),
    ('Giyim & Moda', 'giyim-moda', 'Kadın, erkek ve çocuk giyim ürünleri', 1, 1, 2, GETUTCDATE()),
    ('Ev & Yaşam', 'ev-yasam', 'Ev dekorasyon ve yaşam ürünleri', 1, 1, 3, GETUTCDATE()),
    ('Spor & Outdoor', 'spor-outdoor', 'Spor ekipmanları ve outdoor ürünler', 1, 0, 4, GETUTCDATE()),
    ('Kitap & Hobi', 'kitap-hobi', 'Kitaplar ve hobi malzemeleri', 1, 0, 5, GETUTCDATE());
END

-- Alt kategoriler
DECLARE @elektronik_id BIGINT = (SELECT id FROM categories WHERE slug = 'elektronik');
DECLARE @giyim_id BIGINT = (SELECT id FROM categories WHERE slug = 'giyim-moda');
DECLARE @ev_id BIGINT = (SELECT id FROM categories WHERE slug = 'ev-yasam');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'telefon-aksesuar')
BEGIN
    INSERT INTO categories (parent_id, name, slug, description, is_active, display_order, created_at)
    VALUES 
    (@elektronik_id, 'Telefon & Aksesuar', 'telefon-aksesuar', 'Telefonlar ve aksesuarları', 1, 1, GETUTCDATE()),
    (@elektronik_id, 'Bilgisayar & Tablet', 'bilgisayar-tablet', 'Bilgisayar ve tablet ürünleri', 1, 2, GETUTCDATE()),
    (@elektronik_id, 'TV & Ses Sistemleri', 'tv-ses-sistemleri', 'Televizyon ve ses sistemleri', 1, 3, GETUTCDATE()),
    (@giyim_id, 'Kadın Giyim', 'kadin-giyim', 'Kadın giyim ürünleri', 1, 1, GETUTCDATE()),
    (@giyim_id, 'Erkek Giyim', 'erkek-giyim', 'Erkek giyim ürünleri', 1, 2, GETUTCDATE()),
    (@giyim_id, 'Çocuk Giyim', 'cocuk-giyim', 'Çocuk giyim ürünleri', 1, 3, GETUTCDATE()),
    (@ev_id, 'Mobilya', 'mobilya', 'Ev mobilyaları', 1, 1, GETUTCDATE()),
    (@ev_id, 'Dekorasyon', 'dekorasyon', 'Ev dekorasyon ürünleri', 1, 2, GETUTCDATE()),
    (@ev_id, 'Mutfak & Banyo', 'mutfak-banyo', 'Mutfak ve banyo ürünleri', 1, 3, GETUTCDATE());
END

-- 3. Test Sellers and Stores
IF NOT EXISTS (SELECT 1 FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local'))
BEGIN
    INSERT INTO sellers (user_id, commission_rate, is_active, created_at)
    VALUES ((SELECT id FROM app_users WHERE email = 'seller@marketplace.local'), 10.00, 1, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM stores WHERE slug = 'techstore')
BEGIN
    INSERT INTO stores (seller_id, name, slug, description, is_active, created_at)
    VALUES ((SELECT id FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local')), 'TechStore', 'techstore', 'Teknoloji ürünleri mağazası', 1, GETUTCDATE());
END

-- Store Categories bağlantısı
DECLARE @store_id BIGINT = (SELECT id FROM stores WHERE slug = 'techstore');
DECLARE @telefon_cat_id BIGINT = (SELECT id FROM categories WHERE slug = 'telefon-aksesuar');
DECLARE @bilgisayar_cat_id BIGINT = (SELECT id FROM categories WHERE slug = 'bilgisayar-tablet');
DECLARE @tv_cat_id BIGINT = (SELECT id FROM categories WHERE slug = 'tv-ses-sistemleri');

IF NOT EXISTS (SELECT 1 FROM store_categories WHERE store_id = @store_id AND category_id = @elektronik_id)
BEGIN
    INSERT INTO store_categories (store_id, category_id, is_active, display_order, created_at)
    VALUES 
    (@store_id, @elektronik_id, 1, 1, GETUTCDATE()),
    (@store_id, @telefon_cat_id, 1, 2, GETUTCDATE()),
    (@store_id, @bilgisayar_cat_id, 1, 3, GETUTCDATE()),
    (@store_id, @tv_cat_id, 1, 4, GETUTCDATE());
END

-- 4. Test Products
IF NOT EXISTS (SELECT 1 FROM products WHERE slug = 'iphone-15-pro')
BEGIN
    INSERT INTO products (seller_id, category_id, store_id, name, slug, description, short_description, price, compare_at_price, currency, stock_qty, is_active, is_featured, is_published, weight, min_order_qty, max_order_qty, created_at)
    VALUES 
    ((SELECT id FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local')), @telefon_cat_id, @store_id, 'iPhone 15 Pro', 'iphone-15-pro', 'Apple iPhone 15 Pro 128GB Titanium', 'iPhone 15 Pro 128GB', 89999.00, 94999.00, 'TRY', 50, 1, 1, 1, 187, 1, 5, GETUTCDATE()),
    ((SELECT id FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local')), @telefon_cat_id, @store_id, 'Samsung Galaxy S24', 'samsung-galaxy-s24', 'Samsung Galaxy S24 256GB Phantom Black', 'Galaxy S24 256GB', 79999.00, 84999.00, 'TRY', 45, 1, 1, 1, 168, 1, 5, GETUTCDATE()),
    ((SELECT id FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local')), @bilgisayar_cat_id, @store_id, 'MacBook Air M2', 'macbook-air-m2', 'Apple MacBook Air M2 13.6" 256GB', 'MacBook Air M2 13.6"', 129999.00, 139999.00, 'TRY', 25, 1, 1, 1, 1247, 1, 3, GETUTCDATE()),
    ((SELECT id FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local')), @bilgisayar_cat_id, @store_id, 'Dell XPS 13', 'dell-xps-13', 'Dell XPS 13 13.4" Intel i7 512GB', 'Dell XPS 13 13.4"', 89999.00, 94999.00, 'TRY', 30, 1, 0, 1, 1150, 1, 3, GETUTCDATE()),
    ((SELECT id FROM sellers WHERE user_id = (SELECT id FROM app_users WHERE email = 'seller@marketplace.local')), @tv_cat_id, @store_id, 'Samsung 65" QLED TV', 'samsung-65-qled-tv', 'Samsung 65" QLED 4K Smart TV', '65" QLED 4K TV', 89999.00, 99999.00, 'TRY', 15, 1, 1, 1, 25000, 1, 2, GETUTCDATE());
END

-- 5. Test Product Variants
DECLARE @iphone_id BIGINT = (SELECT id FROM products WHERE slug = 'iphone-15-pro');
DECLARE @samsung_id BIGINT = (SELECT id FROM products WHERE slug = 'samsung-galaxy-s24');
DECLARE @macbook_id BIGINT = (SELECT id FROM products WHERE slug = 'macbook-air-m2');

IF NOT EXISTS (SELECT 1 FROM product_variants WHERE sku = 'IP15P-128-TIT')
BEGIN
    INSERT INTO product_variants (product_id, sku, variant_name, price, compare_at_price, stock_qty, is_default, weight, created_at)
    VALUES 
    (@iphone_id, 'IP15P-128-TIT', '128GB Titanium', 89999.00, 94999.00, 20, 1, 187, GETUTCDATE()),
    (@iphone_id, 'IP15P-256-TIT', '256GB Titanium', 99999.00, 104999.00, 15, 0, 187, GETUTCDATE()),
    (@iphone_id, 'IP15P-512-TIT', '512GB Titanium', 114999.00, 119999.00, 10, 0, 187, GETUTCDATE()),
    (@iphone_id, 'IP15P-1TB-TIT', '1TB Titanium', 129999.00, 134999.00, 5, 0, 187, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM product_variants WHERE sku = 'SGS24-128-BLK')
BEGIN
    INSERT INTO product_variants (product_id, sku, variant_name, price, compare_at_price, stock_qty, is_default, weight, created_at)
    VALUES 
    (@samsung_id, 'SGS24-128-BLK', '128GB Phantom Black', 79999.00, 84999.00, 20, 1, 168, GETUTCDATE()),
    (@samsung_id, 'SGS24-256-BLK', '256GB Phantom Black', 89999.00, 94999.00, 15, 0, 168, GETUTCDATE()),
    (@samsung_id, 'SGS24-512-BLK', '512GB Phantom Black', 99999.00, 104999.00, 10, 0, 168, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM product_variants WHERE sku = 'MBA-M2-256')
BEGIN
    INSERT INTO product_variants (product_id, sku, variant_name, price, compare_at_price, stock_qty, is_default, weight, created_at)
    VALUES 
    (@macbook_id, 'MBA-M2-256', '256GB SSD', 129999.00, 139999.00, 15, 1, 1247, GETUTCDATE()),
    (@macbook_id, 'MBA-M2-512', '512GB SSD', 144999.00, 154999.00, 10, 0, 1247, GETUTCDATE());
END

-- 6. Test Product Images
IF NOT EXISTS (SELECT 1 FROM product_images WHERE product_id = @iphone_id AND is_primary = 1)
BEGIN
    INSERT INTO product_images (product_id, image_url, thumbnail_url, alt_text, title, display_order, is_primary, is_active, created_at)
    VALUES 
    (@iphone_id, 'https://example.com/images/iphone15pro-1.jpg', 'https://example.com/images/iphone15pro-1-thumb.jpg', 'iPhone 15 Pro Titanium', 'iPhone 15 Pro Ana Resim', 1, 1, 1, GETUTCDATE()),
    (@iphone_id, 'https://example.com/images/iphone15pro-2.jpg', 'https://example.com/images/iphone15pro-2-thumb.jpg', 'iPhone 15 Pro Arka', 'iPhone 15 Pro Arka Görünüm', 2, 0, 1, GETUTCDATE()),
    (@iphone_id, 'https://example.com/images/iphone15pro-3.jpg', 'https://example.com/images/iphone15pro-3-thumb.jpg', 'iPhone 15 Pro Yan', 'iPhone 15 Pro Yan Görünüm', 3, 0, 1, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM product_images WHERE product_id = @samsung_id AND is_primary = 1)
BEGIN
    INSERT INTO product_images (product_id, image_url, thumbnail_url, alt_text, title, display_order, is_primary, is_active, created_at)
    VALUES 
    (@samsung_id, 'https://example.com/images/s24-1.jpg', 'https://example.com/images/s24-1-thumb.jpg', 'Samsung Galaxy S24', 'Galaxy S24 Ana Resim', 1, 1, 1, GETUTCDATE()),
    (@samsung_id, 'https://example.com/images/s24-2.jpg', 'https://example.com/images/s24-2-thumb.jpg', 'Samsung Galaxy S24 Arka', 'Galaxy S24 Arka Görünüm', 2, 0, 1, GETUTCDATE());
END

IF NOT EXISTS (SELECT 1 FROM product_images WHERE product_id = @macbook_id AND is_primary = 1)
BEGIN
    INSERT INTO product_images (product_id, image_url, thumbnail_url, alt_text, title, display_order, is_primary, is_active, created_at)
    VALUES 
    (@macbook_id, 'https://example.com/images/mba-m2-1.jpg', 'https://example.com/images/mba-m2-1-thumb.jpg', 'MacBook Air M2', 'MacBook Air M2 Ana Resim', 1, 1, 1, GETUTCDATE()),
    (@macbook_id, 'https://example.com/images/mba-m2-2.jpg', 'https://example.com/images/mba-m2-2-thumb.jpg', 'MacBook Air M2 Açık', 'MacBook Air M2 Açık Görünüm', 2, 0, 1, GETUTCDATE());
END

-- 7. Test Store Applications
IF NOT EXISTS (SELECT 1 FROM store_applications WHERE store_name = 'Fashion Boutique')
BEGIN
    INSERT INTO store_applications (user_id, store_name, store_description, business_type, tax_number, phone, address, status, created_at)
    VALUES 
    ((SELECT id FROM app_users WHERE email = 'seller@marketplace.local'), 'Fashion Boutique', 'Kadın giyim ve aksesuar mağazası', 'Giyim', '1234567890', '+90 555 123 4567', 'İstanbul, Türkiye', 'Approved', GETUTCDATE()),
    ((SELECT id FROM app_users WHERE email = 'customer@marketplace.local'), 'Home & Garden', 'Ev dekorasyon ve bahçe ürünleri', 'Ev & Yaşam', '0987654321', '+90 555 987 6543', 'Ankara, Türkiye', 'Pending', GETUTCDATE());
END

-- Test verilerini doğrula
PRINT '=== Test Verileri Eklendi ===';
PRINT 'Kullanıcılar: ' + CAST((SELECT COUNT(*) FROM app_users) AS VARCHAR) + ' adet';
PRINT 'Kategoriler: ' + CAST((SELECT COUNT(*) FROM categories) AS VARCHAR) + ' adet';
PRINT 'Satıcılar: ' + CAST((SELECT COUNT(*) FROM sellers) AS VARCHAR) + ' adet';
PRINT 'Mağazalar: ' + CAST((SELECT COUNT(*) FROM stores) AS VARCHAR) + ' adet';
PRINT 'Ürünler: ' + CAST((SELECT COUNT(*) FROM products) AS VARCHAR) + ' adet';
PRINT 'Varyantlar: ' + CAST((SELECT COUNT(*) FROM product_variants) AS VARCHAR) + ' adet';
PRINT 'Resimler: ' + CAST((SELECT COUNT(*) FROM product_images) AS VARCHAR) + ' adet';
PRINT 'Mağaza Başvuruları: ' + CAST((SELECT COUNT(*) FROM store_applications) AS VARCHAR) + ' adet';

-- Özet rapor
SELECT 
    'Kategoriler' as entity_type,
    COUNT(*) as count,
    'Ana kategoriler ve alt kategoriler' as description
FROM categories
UNION ALL
SELECT 
    'Ürünler' as entity_type,
    COUNT(*) as count,
    'Elektronik ürünler (telefon, bilgisayar, TV)' as description
FROM products
UNION ALL
SELECT 
    'Varyantlar' as entity_type,
    COUNT(*) as count,
    'Ürün varyantları (farklı özellikler)' as description
FROM product_variants
UNION ALL
SELECT 
    'Resimler' as entity_type,
    COUNT(*) as count,
    'Ürün resimleri (ana ve ikincil)' as description
FROM product_images;
