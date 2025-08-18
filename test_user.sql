-- Test Admin Kullanıcısı Oluşturma
-- Şifre: admin123 (SHA256 hash: 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9)

INSERT INTO app_users (email, full_name, password_hash, role, is_active, created_at, modified_at)
VALUES (
    'admin@marketplace.local',
    'Admin User',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'Admin',
    1,
    GETDATE(),
    GETDATE()
);

-- Test Seller Kullanıcısı
INSERT INTO app_users (email, full_name, password_hash, role, is_active, created_at, modified_at)
VALUES (
    'seller@marketplace.local',
    'Test Seller',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'Seller',
    1,
    GETDATE(),
    GETDATE()
);

-- Test Customer Kullanıcısı
INSERT INTO app_users (email, full_name, password_hash, role, is_active, created_at, modified_at)
VALUES (
    'customer@marketplace.local',
    'Test Customer',
    '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
    'Customer',
    1,
    GETDATE(),
    GETDATE()
);
