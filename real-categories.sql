-- Marketplace Gerçek Kategoriler Ekleme Scripti
-- Bu script'i SQL Server Management Studio'da çalıştırın

USE [Marketplace];
GO

-- Mevcut test kategorilerini temizle (isteğe bağlı)
-- DELETE FROM categories WHERE slug IN ('test-category', 'elektronik', 'giyim-moda', 'ev-yasam', 'spor-outdoor', 'kitap-hobi');

-- Ana Kategoriler (Parent kategoriler)
PRINT 'Ana kategoriler ekleniyor...';

-- 1. Elektronik & Teknoloji
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'elektronik-teknoloji')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Elektronik & Teknoloji', 'elektronik-teknoloji', 
        'Telefon, bilgisayar, tablet ve tüm teknoloji ürünleri', 
        1, 1, 1, 
        'https://images.example.com/categories/electronics.jpg',
        'bi-laptop',
        'Elektronik Ürünler - Teknoloji Mağazası',
        'En yeni teknoloji ürünleri, telefonlar, bilgisayarlar ve elektronik aksesuarlar',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 2. Giyim & Moda
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'giyim-moda')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Giyim & Moda', 'giyim-moda', 
        'Kadın, erkek ve çocuk giyim ürünleri', 
        1, 1, 2, 
        'https://images.example.com/categories/fashion.jpg',
        'bi-shop',
        'Giyim & Moda - Trend Kıyafetler',
        'En trend kadın, erkek ve çocuk giyim ürünleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 3. Ev & Yaşam
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'ev-yasam')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Ev & Yaşam', 'ev-yasam', 
        'Ev dekorasyonu, mobilya ve yaşam ürünleri', 
        1, 1, 3, 
        'https://images.example.com/categories/home.jpg',
        'bi-house',
        'Ev & Yaşam - Dekorasyon ve Mobilya',
        'Ev dekorasyonu, mobilya ve yaşam kalitesini artıran ürünler',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 4. Spor & Outdoor
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'spor-outdoor')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Spor & Outdoor', 'spor-outdoor', 
        'Spor ekipmanları, outdoor ürünler ve fitness', 
        1, 1, 4, 
        'https://images.example.com/categories/sports.jpg',
        'bi-bicycle',
        'Spor & Outdoor - Fitness ve Doğa',
        'Spor ekipmanları, outdoor ürünler ve fitness malzemeleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 5. Kozmetik & Kişisel Bakım
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'kozmetik-kisisel-bakim')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Kozmetik & Kişisel Bakım', 'kozmetik-kisisel-bakim', 
        'Makyaj, cilt bakımı ve kişisel bakım ürünleri', 
        1, 1, 5, 
        'https://images.example.com/categories/cosmetics.jpg',
        'bi-gem',
        'Kozmetik & Kişisel Bakım - Güzellik Ürünleri',
        'Makyaj, cilt bakımı ve kişisel bakım ürünleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 6. Anne & Bebek
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'anne-bebek')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Anne & Bebek', 'anne-bebek', 
        'Bebek ürünleri, anne bakımı ve çocuk eşyaları', 
        1, 1, 6, 
        'https://images.example.com/categories/baby.jpg',
        'bi-heart',
        'Anne & Bebek - Bebek Ürünleri',
        'Bebek bakımı, anne ürünleri ve çocuk eşyaları',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 7. Kitap & Hobi
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'kitap-hobi')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Kitap & Hobi', 'kitap-hobi', 
        'Kitaplar, hobi malzemeleri ve sanat ürünleri', 
        1, 0, 7, 
        'https://images.example.com/categories/books.jpg',
        'bi-book',
        'Kitap & Hobi - Okuma ve Sanat',
        'Kitaplar, hobi malzemeleri ve sanat ürünleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 8. Otomotiv
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'otomotiv')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Otomotiv', 'otomotiv', 
        'Araç aksesuarları, yedek parça ve otomotiv ürünleri', 
        1, 0, 8, 
        'https://images.example.com/categories/automotive.jpg',
        'bi-car-front',
        'Otomotiv - Araç Aksesuarları',
        'Araç aksesuarları, yedek parça ve otomotiv ürünleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 9. Pet Shop
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'pet-shop')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Pet Shop', 'pet-shop', 
        'Evcil hayvan ürünleri, mama ve aksesuarlar', 
        1, 0, 9, 
        'https://images.example.com/categories/pets.jpg',
        'bi-heart-pulse',
        'Pet Shop - Evcil Hayvan Ürünleri',
        'Evcil hayvan mama, oyuncak ve bakım ürünleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

-- 10. Süpermarket
IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'supermarket')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, is_featured, 
        display_order, image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES (
        NULL, 'Süpermarket', 'supermarket', 
        'Gıda, temizlik ve günlük ihtiyaç ürünleri', 
        1, 1, 10, 
        'https://images.example.com/categories/supermarket.jpg',
        'bi-cart',
        'Süpermarket - Gıda ve Temizlik',
        'Gıda, temizlik ve günlük ihtiyaç ürünleri',
        GETUTCDATE(), GETUTCDATE()
    );
END

PRINT 'Ana kategoriler eklendi.';

-- Alt Kategoriler (Child kategoriler)
PRINT 'Alt kategoriler ekleniyor...';

-- Elektronik alt kategorileri
DECLARE @elektronik_id BIGINT = (SELECT id FROM categories WHERE slug = 'elektronik-teknoloji');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'telefon-aksesuar')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@elektronik_id, 'Telefon & Aksesuar', 'telefon-aksesuar', 'Akıllı telefonlar ve aksesuarları', 1, 1, 'https://images.example.com/categories/phones.jpg', 'bi-phone', 'Telefon & Aksesuar - Akıllı Telefonlar', 'iPhone, Samsung ve diğer marka telefonlar', GETUTCDATE(), GETUTCDATE()),
    (@elektronik_id, 'Bilgisayar & Tablet', 'bilgisayar-tablet', 'Laptop, masaüstü bilgisayar ve tabletler', 1, 2, 'https://images.example.com/categories/computers.jpg', 'bi-laptop', 'Bilgisayar & Tablet - Laptop ve Tablet', 'Laptop, masaüstü bilgisayar ve tablet ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@elektronik_id, 'TV & Ses Sistemleri', 'tv-ses-sistemleri', 'Televizyon, ses sistemi ve projeksiyon', 1, 3, 'https://images.example.com/categories/tv.jpg', 'bi-tv', 'TV & Ses Sistemleri - Televizyon', 'Smart TV, ses sistemi ve projeksiyon cihazları', GETUTCDATE(), GETUTCDATE()),
    (@elektronik_id, 'Kamera & Fotoğraf', 'kamera-fotograf', 'DSLR, aksiyon kameralar ve fotoğraf ekipmanları', 1, 4, 'https://images.example.com/categories/camera.jpg', 'bi-camera', 'Kamera & Fotoğraf - Fotoğraf Makineleri', 'DSLR, aksiyon kameralar ve fotoğraf ekipmanları', GETUTCDATE(), GETUTCDATE()),
    (@elektronik_id, 'Gaming', 'gaming', 'Oyun konsolları, oyun bilgisayarları ve aksesuarlar', 1, 5, 'https://images.example.com/categories/gaming.jpg', 'bi-controller', 'Gaming - Oyun Konsolları', 'PlayStation, Xbox, Nintendo ve gaming aksesuarları', GETUTCDATE(), GETUTCDATE()),
    (@elektronik_id, 'Küçük Ev Aletleri', 'kucuk-ev-aletleri', 'Mutfak robotları, süpürge ve küçük ev aletleri', 1, 6, 'https://images.example.com/categories/appliances.jpg', 'bi-house-gear', 'Küçük Ev Aletleri - Mutfak Robotları', 'Mutfak robotları, süpürge ve küçük ev aletleri', GETUTCDATE(), GETUTCDATE());
END

-- Giyim alt kategorileri
DECLARE @giyim_id BIGINT = (SELECT id FROM categories WHERE slug = 'giyim-moda');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'kadin-giyim')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@giyim_id, 'Kadın Giyim', 'kadin-giyim', 'Kadın kıyafetleri ve aksesuarları', 1, 1, 'https://images.example.com/categories/women-clothing.jpg', 'bi-person-dress', 'Kadın Giyim - Trend Kıyafetler', 'Kadın elbise, bluz, pantolon ve aksesuarları', GETUTCDATE(), GETUTCDATE()),
    (@giyim_id, 'Erkek Giyim', 'erkek-giyim', 'Erkek kıyafetleri ve aksesuarları', 1, 2, 'https://images.example.com/categories/men-clothing.jpg', 'bi-person', 'Erkek Giyim - Erkek Kıyafetleri', 'Erkek gömlek, pantolon, ceket ve aksesuarları', GETUTCDATE(), GETUTCDATE()),
    (@giyim_id, 'Çocuk Giyim', 'cocuk-giyim', 'Bebek ve çocuk kıyafetleri', 1, 3, 'https://images.example.com/categories/kids-clothing.jpg', 'bi-person-heart', 'Çocuk Giyim - Bebek ve Çocuk Kıyafetleri', 'Bebek ve çocuk kıyafetleri, ayakkabı ve aksesuarları', GETUTCDATE(), GETUTCDATE()),
    (@giyim_id, 'Ayakkabı', 'ayakkabi', 'Kadın, erkek ve çocuk ayakkabıları', 1, 4, 'https://images.example.com/categories/shoes.jpg', 'bi-shoe-prints', 'Ayakkabı - Kadın Erkek Çocuk', 'Spor ayakkabı, bot, sandalet ve klasik ayakkabılar', GETUTCDATE(), GETUTCDATE()),
    (@giyim_id, 'Çanta & Aksesuar', 'canta-aksesuar', 'Çanta, saat, takı ve aksesuarlar', 1, 5, 'https://images.example.com/categories/bags.jpg', 'bi-bag', 'Çanta & Aksesuar - Moda Aksesuarları', 'Çanta, saat, takı ve moda aksesuarları', GETUTCDATE(), GETUTCDATE()),
    (@giyim_id, 'İç Giyim', 'ic-giyim', 'İç çamaşırı ve pijama', 1, 6, 'https://images.example.com/categories/underwear.jpg', 'bi-heart', 'İç Giyim - İç Çamaşırı', 'İç çamaşırı, pijama ve ev kıyafetleri', GETUTCDATE(), GETUTCDATE());
END

-- Ev & Yaşam alt kategorileri
DECLARE @ev_id BIGINT = (SELECT id FROM categories WHERE slug = 'ev-yasam');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'mobilya')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@ev_id, 'Mobilya', 'mobilya', 'Yatak odası, oturma odası ve mutfak mobilyaları', 1, 1, 'https://images.example.com/categories/furniture.jpg', 'bi-house-door', 'Mobilya - Ev Mobilyaları', 'Yatak odası, oturma odası ve mutfak mobilyaları', GETUTCDATE(), GETUTCDATE()),
    (@ev_id, 'Dekorasyon', 'dekorasyon', 'Duvar dekorasyonu, aydınlatma ve süs eşyaları', 1, 2, 'https://images.example.com/categories/decoration.jpg', 'bi-palette', 'Dekorasyon - Ev Dekorasyonu', 'Duvar dekorasyonu, aydınlatma ve süs eşyaları', GETUTCDATE(), GETUTCDATE()),
    (@ev_id, 'Mutfak & Banyo', 'mutfak-banyo', 'Mutfak eşyaları, banyo aksesuarları ve tekstil', 1, 3, 'https://images.example.com/categories/kitchen.jpg', 'bi-house-gear', 'Mutfak & Banyo - Mutfak Eşyaları', 'Mutfak eşyaları, banyo aksesuarları ve tekstil ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@ev_id, 'Bahçe & Outdoor', 'bahce-outdoor', 'Bahçe mobilyaları, bitki ve outdoor ürünler', 1, 4, 'https://images.example.com/categories/garden.jpg', 'bi-tree', 'Bahçe & Outdoor - Bahçe Mobilyaları', 'Bahçe mobilyaları, bitki ve outdoor ürünler', GETUTCDATE(), GETUTCDATE()),
    (@ev_id, 'Yatak Odası', 'yatak-odasi', 'Yatak, yatak takımları ve yatak odası mobilyaları', 1, 5, 'https://images.example.com/categories/bedroom.jpg', 'bi-moon', 'Yatak Odası - Yatak ve Yatak Takımları', 'Yatak, yatak takımları ve yatak odası mobilyaları', GETUTCDATE(), GETUTCDATE()),
    (@ev_id, 'Çalışma Odası', 'calisma-odasi', 'Masa, sandalye ve ofis mobilyaları', 1, 6, 'https://images.example.com/categories/office.jpg', 'bi-laptop', 'Çalışma Odası - Ofis Mobilyaları', 'Masa, sandalye ve ofis mobilyaları', GETUTCDATE(), GETUTCDATE());
END

-- Spor & Outdoor alt kategorileri
DECLARE @spor_id BIGINT = (SELECT id FROM categories WHERE slug = 'spor-outdoor');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'fitness-spor')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@spor_id, 'Fitness & Spor', 'fitness-spor', 'Fitness ekipmanları ve spor malzemeleri', 1, 1, 'https://images.example.com/categories/fitness.jpg', 'bi-activity', 'Fitness & Spor - Spor Ekipmanları', 'Fitness ekipmanları ve spor malzemeleri', GETUTCDATE(), GETUTCDATE()),
    (@spor_id, 'Outdoor & Kamp', 'outdoor-kamp', 'Kamp malzemeleri, outdoor ekipmanları', 1, 2, 'https://images.example.com/categories/camping.jpg', 'bi-tree', 'Outdoor & Kamp - Kamp Malzemeleri', 'Kamp malzemeleri, outdoor ekipmanları', GETUTCDATE(), GETUTCDATE()),
    (@spor_id, 'Spor Giyim', 'spor-giyim', 'Spor kıyafetleri ve ayakkabıları', 1, 3, 'https://images.example.com/categories/sportswear.jpg', 'bi-person-running', 'Spor Giyim - Spor Kıyafetleri', 'Spor kıyafetleri ve ayakkabıları', GETUTCDATE(), GETUTCDATE()),
    (@spor_id, 'Su Sporları', 'su-sporlari', 'Yüzme, dalış ve su sporları ekipmanları', 1, 4, 'https://images.example.com/categories/watersports.jpg', 'bi-water', 'Su Sporları - Yüzme Ekipmanları', 'Yüzme, dalış ve su sporları ekipmanları', GETUTCDATE(), GETUTCDATE()),
    (@spor_id, 'Kış Sporları', 'kis-sporlari', 'Kayak, snowboard ve kış sporları', 1, 5, 'https://images.example.com/categories/wintersports.jpg', 'bi-snow', 'Kış Sporları - Kayak Ekipmanları', 'Kayak, snowboard ve kış sporları ekipmanları', GETUTCDATE(), GETUTCDATE());
END

-- Kozmetik & Kişisel Bakım alt kategorileri
DECLARE @kozmetik_id BIGINT = (SELECT id FROM categories WHERE slug = 'kozmetik-kisisel-bakim');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'makyaj')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@kozmetik_id, 'Makyaj', 'makyaj', 'Ruj, fondöten, göz makyajı ve makyaj fırçaları', 1, 1, 'https://images.example.com/categories/makeup.jpg', 'bi-palette', 'Makyaj - Kozmetik Ürünleri', 'Ruj, fondöten, göz makyajı ve makyaj fırçaları', GETUTCDATE(), GETUTCDATE()),
    (@kozmetik_id, 'Cilt Bakımı', 'cilt-bakimi', 'Nemlendirici, temizleyici ve cilt bakım ürünleri', 1, 2, 'https://images.example.com/categories/skincare.jpg', 'bi-droplet', 'Cilt Bakımı - Cilt Bakım Ürünleri', 'Nemlendirici, temizleyici ve cilt bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@kozmetik_id, 'Saç Bakımı', 'sac-bakimi', 'Şampuan, saç kremi ve saç bakım ürünleri', 1, 3, 'https://images.example.com/categories/haircare.jpg', 'bi-scissors', 'Saç Bakımı - Saç Bakım Ürünleri', 'Şampuan, saç kremi ve saç bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@kozmetik_id, 'Parfüm & Deodorant', 'parfum-deodorant', 'Parfüm, deodorant ve vücut bakım ürünleri', 1, 4, 'https://images.example.com/categories/perfume.jpg', 'bi-flower1', 'Parfüm & Deodorant - Parfüm Ürünleri', 'Parfüm, deodorant ve vücut bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@kozmetik_id, 'Erkek Bakım', 'erkek-bakim', 'Erkek cilt bakımı, tıraş ve bakım ürünleri', 1, 5, 'https://images.example.com/categories/menscare.jpg', 'bi-person', 'Erkek Bakım - Erkek Bakım Ürünleri', 'Erkek cilt bakımı, tıraş ve bakım ürünleri', GETUTCDATE(), GETUTCDATE());
END

-- Anne & Bebek alt kategorileri
DECLARE @anne_bebek_id BIGINT = (SELECT id FROM categories WHERE slug = 'anne-bebek');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'bebek-giyim')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@anne_bebek_id, 'Bebek Giyim', 'bebek-giyim', '0-24 ay bebek kıyafetleri', 1, 1, 'https://images.example.com/categories/baby-clothes.jpg', 'bi-heart', 'Bebek Giyim - Bebek Kıyafetleri', '0-24 ay bebek kıyafetleri', GETUTCDATE(), GETUTCDATE()),
    (@anne_bebek_id, 'Bebek Bakımı', 'bebek-bakimi', 'Bebek bezi, bakım ürünleri ve temizlik', 1, 2, 'https://images.example.com/categories/baby-care.jpg', 'bi-droplet', 'Bebek Bakımı - Bebek Bakım Ürünleri', 'Bebek bezi, bakım ürünleri ve temizlik', GETUTCDATE(), GETUTCDATE()),
    (@anne_bebek_id, 'Bebek Beslenme', 'bebek-beslenme', 'Biberon, mama ve beslenme ürünleri', 1, 3, 'https://images.example.com/categories/baby-feeding.jpg', 'bi-cup', 'Bebek Beslenme - Beslenme Ürünleri', 'Biberon, mama ve beslenme ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@anne_bebek_id, 'Bebek Oyuncakları', 'bebek-oyuncaklari', 'Bebek oyuncakları ve eğitici materyaller', 1, 4, 'https://images.example.com/categories/baby-toys.jpg', 'bi-toy', 'Bebek Oyuncakları - Bebek Oyuncakları', 'Bebek oyuncakları ve eğitici materyaller', GETUTCDATE(), GETUTCDATE()),
    (@anne_bebek_id, 'Anne Ürünleri', 'anne-urunleri', 'Hamilelik ve emzirme ürünleri', 1, 5, 'https://images.example.com/categories/mom-products.jpg', 'bi-heart-pulse', 'Anne Ürünleri - Hamilelik Ürünleri', 'Hamilelik ve emzirme ürünleri', GETUTCDATE(), GETUTCDATE());
END

-- Kitap & Hobi alt kategorileri
DECLARE @kitap_id BIGINT = (SELECT id FROM categories WHERE slug = 'kitap-hobi');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'kitaplar')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@kitap_id, 'Kitaplar', 'kitaplar', 'Roman, bilim, çocuk ve eğitim kitapları', 1, 1, 'https://images.example.com/categories/books.jpg', 'bi-book', 'Kitaplar - Kitap Çeşitleri', 'Roman, bilim, çocuk ve eğitim kitapları', GETUTCDATE(), GETUTCDATE()),
    (@kitap_id, 'Hobi Malzemeleri', 'hobi-malzemeleri', 'El sanatları, boyama ve hobi malzemeleri', 1, 2, 'https://images.example.com/categories/hobby.jpg', 'bi-palette', 'Hobi Malzemeleri - El Sanatları', 'El sanatları, boyama ve hobi malzemeleri', GETUTCDATE(), GETUTCDATE()),
    (@kitap_id, 'Müzik & Enstrüman', 'muzik-enstruman', 'Müzik aletleri, CD ve müzik aksesuarları', 1, 3, 'https://images.example.com/categories/music.jpg', 'bi-music-note', 'Müzik & Enstrüman - Müzik Aletleri', 'Müzik aletleri, CD ve müzik aksesuarları', GETUTCDATE(), GETUTCDATE()),
    (@kitap_id, 'Sanat & Tasarım', 'sanat-tasarim', 'Sanat malzemeleri, tasarım ürünleri', 1, 4, 'https://images.example.com/categories/art.jpg', 'bi-brush', 'Sanat & Tasarım - Sanat Malzemeleri', 'Sanat malzemeleri, tasarım ürünleri', GETUTCDATE(), GETUTCDATE());
END

-- Otomotiv alt kategorileri
DECLARE @otomotiv_id BIGINT = (SELECT id FROM categories WHERE slug = 'otomotiv');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'arac-aksesuar')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@otomotiv_id, 'Araç Aksesuar', 'arac-aksesuar', 'Araç içi ve dışı aksesuarlar', 1, 1, 'https://images.example.com/categories/car-accessories.jpg', 'bi-car-front', 'Araç Aksesuar - Araç Aksesuarları', 'Araç içi ve dışı aksesuarlar', GETUTCDATE(), GETUTCDATE()),
    (@otomotiv_id, 'Yedek Parça', 'yedek-parca', 'Motor, fren ve diğer yedek parçalar', 1, 2, 'https://images.example.com/categories/spare-parts.jpg', 'bi-gear', 'Yedek Parça - Araç Yedek Parçaları', 'Motor, fren ve diğer yedek parçalar', GETUTCDATE(), GETUTCDATE()),
    (@otomotiv_id, 'Araç Bakım', 'arac-bakim', 'Yağ, filtre ve bakım ürünleri', 1, 3, 'https://images.example.com/categories/car-care.jpg', 'bi-tools', 'Araç Bakım - Araç Bakım Ürünleri', 'Yağ, filtre ve bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@otomotiv_id, 'Motorsiklet', 'motorsiklet', 'Motorsiklet aksesuar ve yedek parçaları', 1, 4, 'https://images.example.com/categories/motorcycle.jpg', 'bi-bicycle', 'Motorsiklet - Motorsiklet Aksesuarları', 'Motorsiklet aksesuar ve yedek parçaları', GETUTCDATE(), GETUTCDATE());
END

-- Pet Shop alt kategorileri
DECLARE @pet_id BIGINT = (SELECT id FROM categories WHERE slug = 'pet-shop');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'kopek-urunleri')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@pet_id, 'Köpek Ürünleri', 'kopek-urunleri', 'Köpek mama, oyuncak ve bakım ürünleri', 1, 1, 'https://images.example.com/categories/dog-products.jpg', 'bi-heart-pulse', 'Köpek Ürünleri - Köpek Bakım', 'Köpek mama, oyuncak ve bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@pet_id, 'Kedi Ürünleri', 'kedi-urunleri', 'Kedi mama, oyuncak ve bakım ürünleri', 1, 2, 'https://images.example.com/categories/cat-products.jpg', 'bi-heart', 'Kedi Ürünleri - Kedi Bakım', 'Kedi mama, oyuncak ve bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@pet_id, 'Akvaryum', 'akvaryum', 'Balık, akvaryum ekipmanları ve yem', 1, 3, 'https://images.example.com/categories/aquarium.jpg', 'bi-water', 'Akvaryum - Balık ve Akvaryum', 'Balık, akvaryum ekipmanları ve yem', GETUTCDATE(), GETUTCDATE()),
    (@pet_id, 'Kuş Ürünleri', 'kus-urunleri', 'Kuş kafesi, yem ve kuş bakım ürünleri', 1, 4, 'https://images.example.com/categories/bird-products.jpg', 'bi-bird', 'Kuş Ürünleri - Kuş Bakım', 'Kuş kafesi, yem ve kuş bakım ürünleri', GETUTCDATE(), GETUTCDATE());
END

-- Süpermarket alt kategorileri
DECLARE @supermarket_id BIGINT = (SELECT id FROM categories WHERE slug = 'supermarket');

IF NOT EXISTS (SELECT 1 FROM categories WHERE slug = 'gida-urunleri')
BEGIN
    INSERT INTO categories (
        parent_id, name, slug, description, is_active, display_order, 
        image_url, icon_class, meta_title, meta_description, 
        created_at, modified_at
    ) VALUES 
    (@supermarket_id, 'Gıda Ürünleri', 'gida-urunleri', 'Temel gıda, konserve ve dondurulmuş ürünler', 1, 1, 'https://images.example.com/categories/food.jpg', 'bi-cup', 'Gıda Ürünleri - Temel Gıda', 'Temel gıda, konserve ve dondurulmuş ürünler', GETUTCDATE(), GETUTCDATE()),
    (@supermarket_id, 'İçecek', 'icecek', 'Su, meşrubat, çay ve kahve', 1, 2, 'https://images.example.com/categories/beverages.jpg', 'bi-cup-hot', 'İçecek - Su ve Meşrubat', 'Su, meşrubat, çay ve kahve', GETUTCDATE(), GETUTCDATE()),
    (@supermarket_id, 'Temizlik', 'temizlik', 'Ev temizlik ürünleri ve deterjanlar', 1, 3, 'https://images.example.com/categories/cleaning.jpg', 'bi-house-gear', 'Temizlik - Ev Temizlik Ürünleri', 'Ev temizlik ürünleri ve deterjanlar', GETUTCDATE(), GETUTCDATE()),
    (@supermarket_id, 'Kişisel Bakım', 'kisisel-bakim', 'Şampuan, sabun ve kişisel bakım ürünleri', 1, 4, 'https://images.example.com/categories/personal-care.jpg', 'bi-droplet', 'Kişisel Bakım - Şampuan ve Sabun', 'Şampuan, sabun ve kişisel bakım ürünleri', GETUTCDATE(), GETUTCDATE()),
    (@supermarket_id, 'Bebek & Çocuk', 'bebek-cocuk', 'Bebek bezi, mama ve çocuk ürünleri', 1, 5, 'https://images.example.com/categories/baby-kids.jpg', 'bi-heart', 'Bebek & Çocuk - Bebek Ürünleri', 'Bebek bezi, mama ve çocuk ürünleri', GETUTCDATE(), GETUTCDATE());
END

PRINT 'Alt kategoriler eklendi.';

-- Kategori istatistikleri
PRINT '=== Kategori İstatistikleri ===';
SELECT 
    'Ana Kategoriler' as kategori_tipi,
    COUNT(*) as sayi
FROM categories 
WHERE parent_id IS NULL
UNION ALL
SELECT 
    'Alt Kategoriler' as kategori_tipi,
    COUNT(*) as sayi
FROM categories 
WHERE parent_id IS NOT NULL
UNION ALL
SELECT 
    'Toplam Kategoriler' as kategori_tipi,
    COUNT(*) as sayi
FROM categories;

-- Kategori hiyerarşisi görüntüleme
PRINT '=== Kategori Hiyerarşisi ===';
SELECT 
    c1.name as 'Ana Kategori',
    c2.name as 'Alt Kategori',
    c2.slug as 'Slug',
    c2.is_active as 'Aktif',
    c2.is_featured as 'Öne Çıkan'
FROM categories c1
LEFT JOIN categories c2 ON c1.id = c2.parent_id
WHERE c1.parent_id IS NULL
ORDER BY c1.display_order, c2.display_order;

-- Script tamamlandı mesajları için değişkenler kullan
DECLARE @total_categories INT = (SELECT COUNT(*) FROM categories);
DECLARE @main_categories INT = (SELECT COUNT(*) FROM categories WHERE parent_id IS NULL);
DECLARE @sub_categories INT = (SELECT COUNT(*) FROM categories WHERE parent_id IS NOT NULL);

PRINT '=== Script Tamamlandı ===';
PRINT 'Toplam ' + CAST(@total_categories AS VARCHAR) + ' kategori eklendi.';
PRINT 'Ana kategoriler: ' + CAST(@main_categories AS VARCHAR) + ' adet';
PRINT 'Alt kategoriler: ' + CAST(@sub_categories AS VARCHAR) + ' adet';
