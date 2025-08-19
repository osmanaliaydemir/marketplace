# Marketplace Database Schema

## Tables

### app_users
```sql
CREATE TABLE app_users (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    full_name NVARCHAR(255) NOT NULL,
    role NVARCHAR(50) NOT NULL DEFAULT 'Customer',
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL
);
```

### sellers
```sql
CREATE TABLE sellers (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    user_id BIGINT NOT NULL,
    commission_rate DECIMAL(5,2) NOT NULL DEFAULT 10.00,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (user_id) REFERENCES app_users(id)
);
```

### stores
```sql
CREATE TABLE stores (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    seller_id BIGINT NOT NULL,
    name NVARCHAR(255) NOT NULL,
    slug NVARCHAR(255) NOT NULL UNIQUE,
    logo_url NVARCHAR(500) NULL,
    banner_url NVARCHAR(500) NULL,
    description NVARCHAR(MAX) NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (seller_id) REFERENCES sellers(id)
);
```

### categories
```sql
CREATE TABLE categories (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    parent_id BIGINT NULL,
    name NVARCHAR(255) NOT NULL,
    slug NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(MAX) NULL,
    image_url NVARCHAR(500) NULL,
    icon_class NVARCHAR(100) NULL,
    is_active BIT NOT NULL DEFAULT 1,
    is_featured BIT NOT NULL DEFAULT 0,
    display_order INT NOT NULL DEFAULT 0,
    meta_title NVARCHAR(255) NULL,
    meta_description NVARCHAR(500) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (parent_id) REFERENCES categories(id)
);
```

### products
```sql
CREATE TABLE products (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    seller_id BIGINT NOT NULL,
    category_id BIGINT NOT NULL,
    store_id BIGINT NOT NULL,
    name NVARCHAR(255) NOT NULL,
    slug NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(MAX) NULL,
    short_description NVARCHAR(500) NULL,
    price DECIMAL(10,2) NOT NULL,
    compare_at_price DECIMAL(10,2) NULL,
    currency NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    stock_qty INT NOT NULL DEFAULT 0,
    is_active BIT NOT NULL DEFAULT 1,
    is_featured BIT NOT NULL DEFAULT 0,
    is_published BIT NOT NULL DEFAULT 0,
    weight INT NOT NULL DEFAULT 0,
    min_order_qty INT NULL DEFAULT 1,
    max_order_qty INT NULL,
    meta_title NVARCHAR(255) NULL,
    meta_description NVARCHAR(500) NULL,
    meta_keywords NVARCHAR(500) NULL,
    published_at DATETIME2 NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (seller_id) REFERENCES sellers(id),
    FOREIGN KEY (category_id) REFERENCES categories(id),
    FOREIGN KEY (store_id) REFERENCES stores(id)
);
```

### product_variants
```sql
CREATE TABLE product_variants (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL,
    sku NVARCHAR(50) NULL,
    barcode NVARCHAR(50) NULL,
    variant_name NVARCHAR(100) NULL,
    price DECIMAL(10,2) NOT NULL,
    compare_at_price DECIMAL(10,2) NULL,
    stock_qty INT NOT NULL DEFAULT 0,
    reserved_qty INT NOT NULL DEFAULT 0,
    min_order_qty INT NULL DEFAULT 1,
    max_order_qty INT NULL,
    display_order INT NOT NULL DEFAULT 0,
    is_active BIT NOT NULL DEFAULT 1,
    is_default BIT NOT NULL DEFAULT 0,
    weight INT NOT NULL DEFAULT 0,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (product_id) REFERENCES products(id)
);
```

### product_images
```sql
CREATE TABLE product_images (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL,
    image_url NVARCHAR(500) NOT NULL,
    thumbnail_url NVARCHAR(500) NULL,
    alt_text NVARCHAR(200) NULL,
    title NVARCHAR(100) NULL,
    display_order INT NOT NULL DEFAULT 0,
    is_primary BIT NOT NULL DEFAULT 0,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (product_id) REFERENCES products(id)
);
```

### store_applications
```sql
CREATE TABLE store_applications (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    user_id BIGINT NOT NULL,
    store_name NVARCHAR(255) NOT NULL,
    store_description NVARCHAR(MAX) NULL,
    business_type NVARCHAR(100) NULL,
    tax_number NVARCHAR(50) NULL,
    phone NVARCHAR(20) NULL,
    address NVARCHAR(MAX) NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    rejection_reason NVARCHAR(MAX) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (user_id) REFERENCES app_users(id)
);
```

### orders
```sql
CREATE TABLE orders (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NOT NULL,
    order_number NVARCHAR(50) NOT NULL UNIQUE,
    total_amount DECIMAL(10,2) NOT NULL,
    currency NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    payment_status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    shipping_address NVARCHAR(MAX) NOT NULL,
    billing_address NVARCHAR(MAX) NOT NULL,
    notes NVARCHAR(MAX) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (customer_id) REFERENCES app_users(id)
);
```

### order_items
```sql
CREATE TABLE order_items (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    order_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    product_variant_id BIGINT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (product_id) REFERENCES products(id),
    FOREIGN KEY (product_variant_id) REFERENCES product_variants(id)
);
```

### order_groups
```sql
CREATE TABLE order_groups (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    order_id BIGINT NOT NULL,
    seller_id BIGINT NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    shipping_cost DECIMAL(10,2) NOT NULL DEFAULT 0,
    tax_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    total_amount DECIMAL(10,2) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (seller_id) REFERENCES sellers(id)
);
```

### payments
```sql
CREATE TABLE payments (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    order_id BIGINT NOT NULL,
    payment_method NVARCHAR(50) NOT NULL,
    transaction_id NVARCHAR(100) NULL,
    amount DECIMAL(10,2) NOT NULL,
    currency NVARCHAR(3) NOT NULL DEFAULT 'TRY',
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    gateway_response NVARCHAR(MAX) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id)
);
```

### payment_splits
```sql
CREATE TABLE payment_splits (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    payment_id BIGINT NOT NULL,
    seller_id BIGINT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    commission_amount DECIMAL(10,2) NOT NULL,
    seller_amount DECIMAL(10,2) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (payment_id) REFERENCES payments(id),
    FOREIGN KEY (seller_id) REFERENCES sellers(id)
);
```

### inventory
```sql
CREATE TABLE inventory (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL,
    product_variant_id BIGINT NULL,
    quantity INT NOT NULL DEFAULT 0,
    reserved_quantity INT NOT NULL DEFAULT 0,
    available_quantity AS (quantity - reserved_quantity),
    low_stock_threshold INT NOT NULL DEFAULT 10,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (product_id) REFERENCES products(id),
    FOREIGN KEY (product_variant_id) REFERENCES product_variants(id)
);
```

### ledger_transactions
```sql
CREATE TABLE ledger_transactions (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    reference_type NVARCHAR(50) NOT NULL,
    reference_id BIGINT NOT NULL,
    transaction_date DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    description NVARCHAR(255) NOT NULL,
    total_amount DECIMAL(10,2) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL
);
```

### ledger_postings
```sql
CREATE TABLE ledger_postings (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    transaction_id BIGINT NOT NULL,
    account_code NVARCHAR(20) NOT NULL,
    debit_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    credit_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    description NVARCHAR(255) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (transaction_id) REFERENCES ledger_transactions(id)
);
```

### store_categories
```sql
CREATE TABLE store_categories (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    store_id BIGINT NOT NULL,
    category_id BIGINT NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    display_order INT NOT NULL DEFAULT 0,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (store_id) REFERENCES stores(id),
    FOREIGN KEY (category_id) REFERENCES categories(id)
);
```

### outbox_messages
```sql
CREATE TABLE outbox_messages (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    message_type NVARCHAR(100) NOT NULL,
    message_data NVARCHAR(MAX) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    retry_count INT NOT NULL DEFAULT 0,
    next_retry_at DATETIME2 NULL,
    processed_at DATETIME2 NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

### webhook_deliveries
```sql
CREATE TABLE webhook_deliveries (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    webhook_url NVARCHAR(500) NOT NULL,
    payload NVARCHAR(MAX) NOT NULL,
    response_status INT NULL,
    response_body NVARCHAR(MAX) NULL,
    retry_count INT NOT NULL DEFAULT 0,
    next_retry_at DATETIME2 NULL,
    delivered_at DATETIME2 NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

### refunds
```sql
CREATE TABLE refunds (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    order_id BIGINT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    reason NVARCHAR(255) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    processed_at DATETIME2 NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id)
);
```

### refund_items
```sql
CREATE TABLE refund_items (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    refund_id BIGINT NOT NULL,
    order_item_id BIGINT NOT NULL,
    quantity INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (refund_id) REFERENCES refunds(id),
    FOREIGN KEY (order_item_id) REFERENCES order_items(id)
);
```

### shipments
```sql
CREATE TABLE shipments (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    order_id BIGINT NOT NULL,
    tracking_number NVARCHAR(100) NULL,
    carrier NVARCHAR(100) NULL,
    shipping_method NVARCHAR(100) NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    shipped_at DATETIME2 NULL,
    delivered_at DATETIME2 NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    modified_at DATETIME2 NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id)
);
```

## Test Data Insertion

### 1. Test Users
```sql
-- Admin user
INSERT INTO app_users (email, password_hash, full_name, role, is_active, created_at)
VALUES ('admin@marketplace.local', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Admin User', 'Admin', 1, GETUTCDATE());

-- Seller user
INSERT INTO app_users (email, password_hash, full_name, role, is_active, created_at)
VALUES ('seller@marketplace.local', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Seller User', 'Seller', 1, GETUTCDATE());

-- Customer user
INSERT INTO app_users (email, password_hash, full_name, role, is_active, created_at)
VALUES ('customer@marketplace.local', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'Customer User', 'Customer', 1, GETUTCDATE());

-- Verify users
SELECT id, email, full_name, role, is_active FROM app_users;
```

### 2. Test Categories
```sql
-- Ana kategoriler
INSERT INTO categories (name, slug, description, is_active, is_featured, display_order, created_at)
VALUES 
('Elektronik', 'elektronik', 'Elektronik ürünler ve aksesuarlar', 1, 1, 1, GETUTCDATE()),
('Giyim & Moda', 'giyim-moda', 'Kadın, erkek ve çocuk giyim ürünleri', 1, 1, 2, GETUTCDATE()),
('Ev & Yaşam', 'ev-yasam', 'Ev dekorasyon ve yaşam ürünleri', 1, 1, 3, GETUTCDATE()),
('Spor & Outdoor', 'spor-outdoor', 'Spor ekipmanları ve outdoor ürünler', 1, 0, 4, GETUTCDATE()),
('Kitap & Hobi', 'kitap-hobi', 'Kitaplar ve hobi malzemeleri', 1, 0, 5, GETUTCDATE());

-- Alt kategoriler
INSERT INTO categories (parent_id, name, slug, description, is_active, display_order, created_at)
VALUES 
-- Elektronik alt kategorileri
(1, 'Telefon & Aksesuar', 'telefon-aksesuar', 'Telefonlar ve aksesuarları', 1, 1, GETUTCDATE()),
(1, 'Bilgisayar & Tablet', 'bilgisayar-tablet', 'Bilgisayar ve tablet ürünleri', 1, 2, GETUTCDATE()),
(1, 'TV & Ses Sistemleri', 'tv-ses-sistemleri', 'Televizyon ve ses sistemleri', 1, 3, GETUTCDATE()),

-- Giyim alt kategorileri
(2, 'Kadın Giyim', 'kadin-giyim', 'Kadın giyim ürünleri', 1, 1, GETUTCDATE()),
(2, 'Erkek Giyim', 'erkek-giyim', 'Erkek giyim ürünleri', 1, 2, GETUTCDATE()),
(2, 'Çocuk Giyim', 'cocuk-giyim', 'Çocuk giyim ürünleri', 1, 3, GETUTCDATE()),

-- Ev & Yaşam alt kategorileri
(3, 'Mobilya', 'mobilya', 'Ev mobilyaları', 1, 1, GETUTCDATE()),
(3, 'Dekorasyon', 'dekorasyon', 'Ev dekorasyon ürünleri', 1, 2, GETUTCDATE()),
(3, 'Mutfak & Banyo', 'mutfak-banyo', 'Mutfak ve banyo ürünleri', 1, 3, GETUTCDATE());

-- Verify categories
SELECT id, name, slug, parent_id, is_active, is_featured FROM categories ORDER BY display_order, name;
```

### 3. Test Sellers and Stores
```sql
-- Seller oluştur
INSERT INTO sellers (user_id, commission_rate, is_active, created_at)
VALUES (2, 10.00, 1, GETUTCDATE());

-- Store oluştur
INSERT INTO stores (seller_id, name, slug, description, is_active, created_at)
VALUES (1, 'TechStore', 'techstore', 'Teknoloji ürünleri mağazası', 1, GETUTCDATE());

-- Store Categories bağlantısı
INSERT INTO store_categories (store_id, category_id, is_active, display_order, created_at)
VALUES 
(1, 1, 1, 1, GETUTCDATE()), -- Elektronik
(1, 6, 1, 2, GETUTCDATE()), -- Telefon & Aksesuar
(1, 7, 1, 3, GETUTCDATE()), -- Bilgisayar & Tablet
(1, 8, 1, 4, GETUTCDATE()); -- TV & Ses Sistemleri

-- Verify sellers and stores
SELECT s.id, s.commission_rate, s.is_active, u.full_name, st.name as store_name
FROM sellers s
JOIN app_users u ON s.user_id = u.id
JOIN stores st ON s.id = st.seller_id;
```

### 4. Test Products
```sql
-- Örnek ürünler
INSERT INTO products (seller_id, category_id, store_id, name, slug, description, short_description, price, compare_at_price, currency, stock_qty, is_active, is_featured, is_published, weight, min_order_qty, max_order_qty, created_at)
VALUES 
-- Elektronik ürünleri
(1, 6, 1, 'iPhone 15 Pro', 'iphone-15-pro', 'Apple iPhone 15 Pro 128GB Titanium', 'iPhone 15 Pro 128GB', 89999.00, 94999.00, 'TRY', 50, 1, 1, 1, 187, 1, 5, GETUTCDATE()),
(1, 6, 1, 'Samsung Galaxy S24', 'samsung-galaxy-s24', 'Samsung Galaxy S24 256GB Phantom Black', 'Galaxy S24 256GB', 79999.00, 84999.00, 'TRY', 45, 1, 1, 1, 168, 1, 5, GETUTCDATE()),
(1, 7, 1, 'MacBook Air M2', 'macbook-air-m2', 'Apple MacBook Air M2 13.6" 256GB', 'MacBook Air M2 13.6"', 129999.00, 139999.00, 'TRY', 25, 1, 1, 1, 1247, 1, 3, GETUTCDATE()),
(1, 7, 1, 'Dell XPS 13', 'dell-xps-13', 'Dell XPS 13 13.4" Intel i7 512GB', 'Dell XPS 13 13.4"', 89999.00, 94999.00, 'TRY', 30, 1, 0, 1, 1150, 1, 3, GETUTCDATE()),
(1, 8, 1, 'Samsung 65" QLED TV', 'samsung-65-qled-tv', 'Samsung 65" QLED 4K Smart TV', '65" QLED 4K TV', 89999.00, 99999.00, 'TRY', 15, 1, 1, 1, 25000, 1, 2, GETUTCDATE());

-- Verify products
SELECT p.id, p.name, p.slug, p.price, p.stock_qty, p.is_active, p.is_published, c.name as category_name, s.name as store_name
FROM products p
JOIN categories c ON p.category_id = c.id
JOIN stores s ON p.store_id = s.id
ORDER BY p.created_at DESC;
```

### 5. Test Product Variants
```sql
-- iPhone 15 Pro varyantları
INSERT INTO product_variants (product_id, sku, variant_name, price, compare_at_price, stock_qty, is_default, weight, created_at)
VALUES 
(1, 'IP15P-128-TIT', '128GB Titanium', 89999.00, 94999.00, 20, 1, 187, GETUTCDATE()),
(1, 'IP15P-256-TIT', '256GB Titanium', 99999.00, 104999.00, 15, 0, 187, GETUTCDATE()),
(1, 'IP15P-512-TIT', '512GB Titanium', 114999.00, 119999.00, 10, 0, 187, GETUTCDATE()),
(1, 'IP15P-1TB-TIT', '1TB Titanium', 129999.00, 134999.00, 5, 0, 187, GETUTCDATE());

-- Samsung Galaxy S24 varyantları
INSERT INTO product_variants (product_id, sku, variant_name, price, compare_at_price, stock_qty, is_default, weight, created_at)
VALUES 
(2, 'SGS24-128-BLK', '128GB Phantom Black', 79999.00, 84999.00, 20, 1, 168, GETUTCDATE()),
(2, 'SGS24-256-BLK', '256GB Phantom Black', 89999.00, 94999.00, 15, 0, 168, GETUTCDATE()),
(2, 'SGS24-512-BLK', '512GB Phantom Black', 99999.00, 104999.00, 10, 0, 168, GETUTCDATE());

-- MacBook Air M2 varyantları
INSERT INTO product_variants (product_id, sku, variant_name, price, compare_at_price, stock_qty, is_default, weight, created_at)
VALUES 
(3, 'MBA-M2-256', '256GB SSD', 129999.00, 139999.00, 15, 1, 1247, GETUTCDATE()),
(3, 'MBA-M2-512', '512GB SSD', 144999.00, 154999.00, 10, 0, 1247, GETUTCDATE());

-- Verify variants
SELECT pv.id, pv.sku, pv.variant_name, pv.price, pv.stock_qty, pv.is_default, p.name as product_name
FROM product_variants pv
JOIN products p ON pv.product_id = p.id
ORDER BY p.name, pv.is_default DESC, pv.price;
```

### 6. Test Product Images
```sql
-- iPhone 15 Pro resimleri
INSERT INTO product_images (product_id, image_url, thumbnail_url, alt_text, title, display_order, is_primary, is_active, created_at)
VALUES 
(1, 'https://example.com/images/iphone15pro-1.jpg', 'https://example.com/images/iphone15pro-1-thumb.jpg', 'iPhone 15 Pro Titanium', 'iPhone 15 Pro Ana Resim', 1, 1, 1, GETUTCDATE()),
(1, 'https://example.com/images/iphone15pro-2.jpg', 'https://example.com/images/iphone15pro-2-thumb.jpg', 'iPhone 15 Pro Arka', 'iPhone 15 Pro Arka Görünüm', 2, 0, 1, GETUTCDATE()),
(1, 'https://example.com/images/iphone15pro-3.jpg', 'https://example.com/images/iphone15pro-3-thumb.jpg', 'iPhone 15 Pro Yan', 'iPhone 15 Pro Yan Görünüm', 3, 0, 1, GETUTCDATE());

-- Samsung Galaxy S24 resimleri
INSERT INTO product_images (product_id, image_url, thumbnail_url, alt_text, title, display_order, is_primary, is_active, created_at)
VALUES 
(2, 'https://example.com/images/s24-1.jpg', 'https://example.com/images/s24-1-thumb.jpg', 'Samsung Galaxy S24', 'Galaxy S24 Ana Resim', 1, 1, 1, GETUTCDATE()),
(2, 'https://example.com/images/s24-2.jpg', 'https://example.com/images/s24-2-thumb.jpg', 'Samsung Galaxy S24 Arka', 'Galaxy S24 Arka Görünüm', 2, 0, 1, GETUTCDATE());

-- MacBook Air M2 resimleri
INSERT INTO product_images (product_id, image_url, thumbnail_url, alt_text, title, display_order, is_primary, is_active, created_at)
VALUES 
(3, 'https://example.com/images/mba-m2-1.jpg', 'https://example.com/images/mba-m2-1-thumb.jpg', 'MacBook Air M2', 'MacBook Air M2 Ana Resim', 1, 1, 1, GETUTCDATE()),
(3, 'https://example.com/images/mba-m2-2.jpg', 'https://example.com/images/mba-m2-2-thumb.jpg', 'MacBook Air M2 Açık', 'MacBook Air M2 Açık Görünüm', 2, 0, 1, GETUTCDATE());

-- Verify images
SELECT pi.id, pi.image_url, pi.alt_text, pi.is_primary, pi.display_order, p.name as product_name
FROM product_images pi
JOIN products p ON pi.product_id = p.id
ORDER BY p.name, pi.display_order;
```

### 7. Test Store Applications
```sql
-- Örnek mağaza başvuruları
INSERT INTO store_applications (user_id, store_name, store_description, business_type, tax_number, phone, address, status, created_at)
VALUES 
(2, 'Fashion Boutique', 'Kadın giyim ve aksesuar mağazası', 'Giyim', '1234567890', '+90 555 123 4567', 'İstanbul, Türkiye', 'Approved', GETUTCDATE()),
(3, 'Home & Garden', 'Ev dekorasyon ve bahçe ürünleri', 'Ev & Yaşam', '0987654321', '+90 555 987 6543', 'Ankara, Türkiye', 'Pending', GETUTCDATE());

-- Verify applications
SELECT sa.id, sa.store_name, sa.business_type, sa.status, u.full_name, u.email
FROM store_applications sa
JOIN app_users u ON sa.user_id = u.id
ORDER BY sa.created_at DESC;
```

## Test Data Verification Queries

### Ürün sayısı ve kategorilere göre dağılım
```sql
SELECT 
    c.name as category_name,
    COUNT(p.id) as product_count,
    AVG(p.price) as avg_price,
    SUM(p.stock_qty) as total_stock
FROM categories c
LEFT JOIN products p ON c.id = p.category_id
WHERE c.is_active = 1
GROUP BY c.id, c.name
ORDER BY product_count DESC;
```

### Mağaza bazında ürün istatistikleri
```sql
SELECT 
    s.name as store_name,
    COUNT(p.id) as product_count,
    COUNT(DISTINCT p.category_id) as category_count,
    AVG(p.price) as avg_price,
    SUM(p.stock_qty) as total_stock
FROM stores s
LEFT JOIN products p ON s.id = p.store_id
WHERE s.is_active = 1
GROUP BY s.id, s.name
ORDER BY product_count DESC;
```

### Varyant sayısı ve stok durumu
```sql
SELECT 
    p.name as product_name,
    COUNT(pv.id) as variant_count,
    SUM(pv.stock_qty) as total_stock,
    MIN(pv.price) as min_price,
    MAX(pv.price) as max_price
FROM products p
LEFT JOIN product_variants pv ON p.id = pv.product_id
WHERE p.is_active = 1
GROUP BY p.id, p.name
ORDER BY variant_count DESC;
```

## Notes
- Tüm test verileri `admin123` şifresi ile oluşturuldu (SHA256 hash)
- Ürün fiyatları TRY cinsinden ve gerçekçi değerler
- Kategoriler hiyerarşik yapıda (ana kategori + alt kategoriler)
- Mağaza-kategori bağlantıları kuruldu
- Varyantlar ve resimler örnek ürünlere eklendi