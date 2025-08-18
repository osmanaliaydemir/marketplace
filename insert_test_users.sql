-- Test Kullanıcıları Ekleme
-- Şifre: admin123 (SHA256 hash: 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9)

-- Test Admin Kullanıcısı
INSERT INTO [Marketplace].[dbo].[app_users] 
([email], [password_hash], [full_name], [role], [created_at], [modified_at], [is_active])
VALUES 
('admin@marketplace.local', 
 '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 
 'Admin User', 
 'Admin', 
 GETDATE(), 
 GETDATE(), 
 1);

-- Test Seller Kullanıcısı
INSERT INTO [Marketplace].[dbo].[app_users] 
([email], [password_hash], [full_name], [role], [created_at], [modified_at], [is_active])
VALUES 
('seller@marketplace.local', 
 '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 
 'Test Seller', 
 'Seller', 
 GETDATE(), 
 GETDATE(), 
 1);

-- Test Customer Kullanıcısı
INSERT INTO [Marketplace].[dbo].[app_users] 
([email], [password_hash], [full_name], [role], [created_at], [modified_at], [is_active])
VALUES 
('customer@marketplace.local', 
 '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 
 'Test Customer', 
 'Customer', 
 GETDATE(), 
 GETDATE(), 
 1);

-- Kontrol için eklenen kullanıcıları listele
SELECT [id], [email], [full_name], [role], [is_active], [created_at] 
FROM [Marketplace].[dbo].[app_users] 
ORDER BY [created_at] DESC;
