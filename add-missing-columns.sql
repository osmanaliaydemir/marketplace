-- Marketplace Categories Table - Missing Columns Fix
-- Bu script'i SQL Server Management Studio'da çalıştırın

USE marketplace;
GO

-- 1. image_url kolonu ekle (eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'categories' AND COLUMN_NAME = 'image_url')
BEGIN
    ALTER TABLE categories ADD image_url NVARCHAR(500) NULL;
    PRINT 'image_url kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'image_url kolonu zaten mevcut.';
END

-- 2. icon_class kolonu ekle (eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'categories' AND COLUMN_NAME = 'icon_class')
BEGIN
    ALTER TABLE categories ADD icon_class NVARCHAR(100) NULL;
    PRINT 'icon_class kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'icon_class kolonu zaten mevcut.';
END

-- 3. meta_title kolonu ekle (eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'categories' AND COLUMN_NAME = 'meta_title')
BEGIN
    ALTER TABLE categories ADD meta_title NVARCHAR(255) NULL;
    PRINT 'meta_title kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'meta_title kolonu zaten mevcut.';
END

-- 4. meta_description kolonu ekle (eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'categories' AND COLUMN_NAME = 'meta_description')
BEGIN
    ALTER TABLE categories ADD meta_description NVARCHAR(500) NULL;
    PRINT 'meta_description kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'meta_description kolonu zaten mevcut.';
END

-- 5. modified_at kolonu ekle (eğer yoksa)
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'categories' AND COLUMN_NAME = 'modified_at')
BEGIN
    ALTER TABLE categories ADD modified_at DATETIME2 NULL;
    PRINT 'modified_at kolonu eklendi.';
END
ELSE
BEGIN
    PRINT 'modified_at kolonu zaten mevcut.';
END

-- 6. Mevcut kategorilerin modified_at değerini güncelle
UPDATE categories SET modified_at = created_at WHERE modified_at IS NULL;
PRINT 'Mevcut kategorilerin modified_at değerleri güncellendi.';

-- 7. Tablo yapısını kontrol et
PRINT '=== Categories Tablosu Yapısı ===';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'categories' 
ORDER BY ORDINAL_POSITION;

-- 8. Örnek kategori ekle (test için)
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'test-category')
BEGIN
    INSERT INTO categories (
        parent_id, 
        name, 
        slug, 
        description, 
        image_url, 
        icon_class, 
        is_active, 
        is_featured, 
        display_order, 
        meta_title, 
        meta_description, 
        created_at, 
        modified_at
    ) VALUES (
        NULL, 
        'Test Kategori', 
        'test-category', 
        'Test amaçlı oluşturulan kategori', 
        'https://example.com/test.jpg', 
        'bi-test', 
        1, 
        0, 
        999, 
        'Test Kategori - Meta Title', 
        'Test amaçlı oluşturulan kategori meta açıklaması', 
        GETUTCDATE(), 
        GETUTCDATE()
    );
    PRINT 'Test kategorisi eklendi.';
END
ELSE
BEGIN
    PRINT 'Test kategorisi zaten mevcut.';
END

PRINT '=== Script Tamamlandı ===';
PRINT 'Artık kategori ekleme işlemi çalışmalı!';
