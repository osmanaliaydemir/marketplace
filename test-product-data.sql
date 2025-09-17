-- Test data for product creation tests
-- Categories
INSERT INTO categories (id, name, slug, description, is_active, created_at) 
VALUES 
(1, 'Elektronik', 'elektronik', 'Elektronik ürünler', 1, GETUTCDATE()),
(2, 'Giyim', 'giyim', 'Giyim ürünleri', 1, GETUTCDATE()),
(3, 'Ev & Yaşam', 'ev-yasam', 'Ev ve yaşam ürünleri', 1, GETUTCDATE())
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- Sellers
INSERT INTO sellers (id, user_id, commission_rate, is_active, created_at)
VALUES 
(1, 1, 5.0, 1, GETUTCDATE())
ON DUPLICATE KEY UPDATE commission_rate = VALUES(commission_rate);

-- Stores
INSERT INTO stores (id, seller_id, name, slug, description, is_active, created_at)
VALUES 
(1, 1, 'Test Mağaza', 'test-magaza', 'Test mağazası', 1, GETUTCDATE())
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- Users (for authentication)
INSERT INTO app_users (id, first_name, last_name, email, password_hash, role, is_active, created_at)
VALUES 
(1, 'Test', 'Seller', 'seller@test.com', '$2a$11$example_hash_here', 'Seller', 1, GETUTCDATE())
ON DUPLICATE KEY UPDATE email = VALUES(email);
