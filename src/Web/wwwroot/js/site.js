// Marketplace Platform JavaScript

// Global Variables
let currentUser = null;
let cartItems = [];
let wishlistItems = [];

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeMarketplace();
    loadUserData();
    setupEventListeners();
    initializeAnimations();
});

// Initialize Marketplace
function initializeMarketplace() {
    console.log('Marketplace Platform initialized');
    
    // Check if user is logged in
    const userToken = localStorage.getItem('userToken');
    if (userToken) {
        currentUser = JSON.parse(localStorage.getItem('userData'));
        updateUserInterface();
    }
    
    // Initialize counters
    updateCartCounter();
    updateWishlistCounter();
}

// Load User Data
function loadUserData() {
    // Load cart items
    const savedCart = localStorage.getItem('cartItems');
    if (savedCart) {
        cartItems = JSON.parse(savedCart);
    }
    
    // Load wishlist items
    const savedWishlist = localStorage.getItem('wishlistItems');
    if (savedWishlist) {
        wishlistItems = JSON.parse(savedWishlist);
    }
}

// Setup Event Listeners
function setupEventListeners() {
    // Store application form
    const storeApplicationForm = document.getElementById('storeApplicationForm');
    if (storeApplicationForm) {
        storeApplicationForm.addEventListener('submit', handleStoreApplication);
    }
    
    // Category cards
    document.querySelectorAll('.category-card').forEach(card => {
        card.addEventListener('click', function() {
            const categoryName = this.querySelector('h6').textContent;
            openCategoryPage(categoryName);
        });
    });
    
    // Store cards
    document.querySelectorAll('.store-card').forEach(card => {
        card.addEventListener('click', function() {
            const storeName = this.querySelector('h6').textContent;
            openStorePage(storeName);
        });
    });
    
    // Process cards
    document.querySelectorAll('.process-card').forEach(card => {
        card.addEventListener('click', function() {
            const stepNumber = this.querySelector('.process-number span').textContent;
            showProcessDetails(stepNumber);
        });
    });
    
    // Testimonial cards
    document.querySelectorAll('.testimonial-card').forEach(card => {
        card.addEventListener('click', function() {
            const authorName = this.querySelector('.testimonial-author h6').textContent;
            showTestimonialDetails(authorName);
        });
    });
    
    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Initialize Animations
function initializeAnimations() {
    // Intersection Observer for fade-in animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in-up');
            }
        });
    }, observerOptions);
    
    // Observe all cards and sections
    document.querySelectorAll('.benefit-card, .category-card, .store-card, .process-card, .testimonial-card').forEach(el => {
        observer.observe(el);
    });
}

// Store Application Handler
function handleStoreApplication(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const applicationData = {
        businessName: formData.get('businessName'),
        category: formData.get('category'),
        description: formData.get('description'),
        contactEmail: formData.get('contactEmail'),
        phoneNumber: formData.get('phoneNumber'),
        website: formData.get('website'),
        socialMedia: formData.get('socialMedia'),
        experience: formData.get('experience'),
        expectedRevenue: formData.get('expectedRevenue'),
        submittedAt: new Date().toISOString()
    };
    
    // Show loading state
    const submitBtn = event.target.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Gönderiliyor...';
    submitBtn.disabled = true;
    
    // Simulate API call
    setTimeout(() => {
        // Save to localStorage for demo
        const applications = JSON.parse(localStorage.getItem('storeApplications') || '[]');
        applications.push(applicationData);
        localStorage.setItem('storeApplications', JSON.stringify(applications));
        
        // Show success message
        showNotification('Başvurunuz başarıyla gönderildi! 24 saat içinde size dönüş yapacağız.', 'success');
        
        // Reset form
        event.target.reset();
        
        // Reset button
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
        
        // Redirect to thank you page or show success modal
        showSuccessModal(applicationData);
    }, 2000);
}

// Show Success Modal
function showSuccessModal(applicationData) {
    const modal = document.createElement('div');
    modal.className = 'modal fade show d-block';
    modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
    modal.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-check-circle me-2"></i>Başvuru Başarılı!
                    </h5>
                    <button type="button" class="btn-close btn-close-white" onclick="this.closest('.modal').remove()"></button>
                </div>
                <div class="modal-body">
                    <p>Mağaza açma başvurunuz başarıyla alındı. Başvuru detaylarınız:</p>
                    <ul class="list-unstyled">
                        <li><strong>İşletme Adı:</strong> ${applicationData.businessName}</li>
                        <li><strong>Kategori:</strong> ${applicationData.category}</li>
                        <li><strong>İletişim:</strong> ${applicationData.contactEmail}</li>
                    </ul>
                    <p class="text-muted">Başvurunuz 24 saat içinde değerlendirilecek ve size e-posta ile bilgi verilecektir.</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-success" onclick="this.closest('.modal').remove()">Tamam</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    
    // Auto-remove after 10 seconds
    setTimeout(() => {
        if (modal.parentNode) {
            modal.remove();
        }
    }, 10000);
}

// Category Page Handler
function openCategoryPage(categoryName) {
    console.log(`Opening category: ${categoryName}`);
    
    // Show category info modal
    showCategoryModal(categoryName);
    
    // In real app, redirect to category page
    // window.location.href = `/categories/${categoryName.toLowerCase().replace(/\s+/g, '-')}`;
}

// Show Category Modal
function showCategoryModal(categoryName) {
    const categoryInfo = {
        'Giyim & Moda': {
            description: 'Giyim, ayakkabı, aksesuar ve moda ürünleri satışı için ideal kategori.',
            requirements: 'Ürün fotoğrafları, detaylı açıklamalar, boyut bilgileri gerekli.',
            commission: '5%',
            avgRevenue: '₺15,000/ay'
        },
        'Elektronik': {
            description: 'Teknoloji ürünleri, aksesuarlar ve elektronik cihazlar.',
            requirements: 'Garanti bilgileri, teknik özellikler, orijinal ürün belgesi.',
            commission: '6%',
            avgRevenue: '₺25,000/ay'
        },
        'Ev & Yaşam': {
            description: 'Ev dekorasyonu, mobilya, mutfak ve yaşam ürünleri.',
            requirements: 'Ürün boyutları, malzeme bilgileri, montaj talimatları.',
            commission: '5%',
            avgRevenue: '₺12,000/ay'
        }
    };
    
    const info = categoryInfo[categoryName] || {
        description: 'Bu kategori hakkında detaylı bilgi için bizimle iletişime geçin.',
        requirements: 'Standart mağaza gereksinimleri uygulanır.',
        commission: '5%',
        avgRevenue: '₺10,000/ay'
    };
    
    const modal = document.createElement('div');
    modal.className = 'modal fade show d-block';
    modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
    modal.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-info-circle me-2"></i>${categoryName} Kategorisi
                    </h5>
                    <button type="button" class="btn-close btn-close-white" onclick="this.closest('.modal').remove()"></button>
                </div>
                <div class="modal-body">
                    <p><strong>Açıklama:</strong> ${info.description}</p>
                    <p><strong>Gereksinimler:</strong> ${info.requirements}</p>
                    <div class="row mt-3">
                        <div class="col-6">
                            <div class="text-center p-3 bg-light rounded">
                                <h6 class="text-primary">Komisyon</h6>
                                <span class="fs-4 fw-bold">${info.commission}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-3 bg-light rounded">
                                <h6 class="text-success">Ortalama Gelir</h6>
                                <span class="fs-4 fw-bold">${info.avgRevenue}</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="/store-applications/apply?category=${encodeURIComponent(categoryName)}" class="btn btn-primary">
                        <i class="fas fa-store me-2"></i>Bu Kategoride Mağaza Aç
                    </a>
                    <button type="button" class="btn btn-secondary" onclick="this.closest('.modal').remove()">Kapat</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
}

// Store Page Handler
function openStorePage(storeName) {
    console.log(`Opening store: ${storeName}`);
    
    // Show store info modal
    showStoreModal(storeName);
    
    // In real app, redirect to store page
    // window.location.href = `/stores/${storeName.toLowerCase().replace(/\s+/g, '-')}`;
}

// Show Store Modal
function showStoreModal(storeName) {
    const storeInfo = {
        'TechStore': {
            category: 'Elektronik',
            rating: '4.9',
            products: '2,500+',
            sales: '15,000+',
            customers: '8,000+',
            joined: '2022',
            description: 'Teknoloji ürünlerinde uzmanlaşmış, güvenilir elektronik mağazası.'
        },
        'FashionHub': {
            category: 'Giyim & Moda',
            rating: '4.7',
            products: '1,800+',
            sales: '12,000+',
            customers: '6,000+',
            joined: '2021',
            description: 'Trend giyim ve aksesuar ürünlerinde öncü moda mağazası.'
        }
    };
    
    const info = storeInfo[storeName] || {
        category: 'Genel',
        rating: '4.5',
        products: '1,000+',
        sales: '5,000+',
        customers: '3,000+',
        joined: '2023',
        description: 'Kaliteli ürünler ve müşteri memnuniyeti odaklı mağaza.'
    };
    
    const modal = document.createElement('div');
    modal.className = 'modal fade show d-block';
    modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
    modal.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-store me-2"></i>${storeName}
                    </h5>
                    <button type="button" class="btn-close btn-close-white" onclick="this.closest('.modal').remove()"></button>
                </div>
                <div class="modal-body">
                    <p class="text-muted">${info.description}</p>
                    <div class="row g-3 mt-3">
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Kategori</small>
                                <span class="fw-bold">${info.category}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Puan</small>
                                <span class="fw-bold text-warning">${info.rating}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Ürün</small>
                                <span class="fw-bold">${info.products}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Satış</small>
                                <span class="fw-bold text-success">${info.sales}</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="/stores/${storeName.toLowerCase().replace(/\s+/g, '-')}" class="btn btn-primary">
                        <i class="fas fa-external-link-alt me-2"></i>Mağazayı Ziyaret Et
                    </a>
                    <button type="button" class="btn btn-secondary" onclick="this.closest('.modal').remove()">Kapat</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
}

// Process Details Handler
function showProcessDetails(stepNumber) {
    const processDetails = {
        '1': {
            title: 'Başvuru Süreci',
            description: 'Mağaza açma formunu doldurun, gerekli belgeleri yükleyin.',
            duration: '5-10 dakika',
            requirements: ['İşletme bilgileri', 'Kategori seçimi', 'İletişim bilgileri']
        },
        '2': {
            title: 'Değerlendirme Süreci',
            description: 'Başvurunuz ekibimiz tarafından detaylı olarak incelenir.',
            duration: '24 saat',
            requirements: ['Belge kontrolü', 'Kategori uygunluğu', 'İşletme geçerliliği']
        },
        '3': {
            title: 'Mağaza Açılışı',
            description: 'Onay sonrası mağazanızı hemen açabilir, ürün ekleyebilirsiniz.',
            duration: 'Anında',
            requirements: ['Ürün ekleme', 'Mağaza tasarımı', 'Fiyatlandırma']
        }
    };
    
    const info = processDetails[stepNumber];
    if (!info) return;
    
    const modal = document.createElement('div');
    modal.className = 'modal fade show d-block';
    modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
    modal.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-info-circle me-2"></i>Adım ${stepNumber}: ${info.title}
                    </h5>
                    <button type="button" class="btn-close btn-close-white" onclick="this.closest('.modal').remove()"></button>
                </div>
                <div class="modal-body">
                    <p>${info.description}</p>
                    <div class="alert alert-info">
                        <i class="fas fa-clock me-2"></i><strong>Süre:</strong> ${info.duration}
                    </div>
                    <h6>Gereksinimler:</h6>
                    <ul>
                        ${info.requirements.map(req => `<li>${req}</li>`).join('')}
                    </ul>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="this.closest('.modal').remove()">Kapat</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
}

// Testimonial Details Handler
function showTestimonialDetails(authorName) {
    const testimonialDetails = {
        'Ahmet Yılmaz': {
            store: 'TechStore',
            category: 'Elektronik',
            experience: '2 yıl',
            revenue: '₺25,000/ay',
            story: 'Marketplace platformumuzda 2 yıldır başarıyla satış yapıyorum. Platform çok kullanıcı dostu ve destek ekibi her zaman yardımcı oluyor.'
        },
        'Ayşe Demir': {
            store: 'FashionHub',
            category: 'Giyim & Moda',
            experience: '1.5 yıl',
            revenue: '₺18,000/ay',
            story: 'Düşük komisyon oranları ve geniş müşteri ağı ile gelirim %300 arttı. Kesinlikle tavsiye ederim!'
        }
    };
    
    const info = testimonialDetails[authorName];
    if (!info) return;
    
    const modal = document.createElement('div');
    modal.className = 'modal fade show d-block';
    modal.style.backgroundColor = 'rgba(0,0,0,0.5)';
    modal.innerHTML = `
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-user me-2"></i>${authorName} - Başarı Hikayesi
                    </h5>
                    <button type="button" class="btn-close btn-close-white" onclick="this.closest('.modal').remove()"></button>
                </div>
                <div class="modal-body">
                    <div class="row g-3 mb-3">
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Mağaza</small>
                                <span class="fw-bold">${info.store}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Kategori</small>
                                <span class="fw-bold">${info.category}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Deneyim</small>
                                <span class="fw-bold">${info.experience}</span>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="text-center p-2 bg-light rounded">
                                <small class="text-muted d-block">Gelir</small>
                                <span class="fw-bold text-success">${info.revenue}</span>
                            </div>
                        </div>
                    </div>
                    <p class="fst-italic">"${info.story}"</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="this.closest('.modal').remove()">Kapat</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
}

// Update User Interface
function updateUserInterface() {
    if (currentUser) {
        // Show user menu
        const userMenu = document.querySelector('.user-menu');
        if (userMenu) {
            userMenu.innerHTML = `
                <div class="dropdown">
                    <button class="btn btn-link dropdown-toggle text-white" type="button" data-bs-toggle="dropdown">
                        <i class="fas fa-user me-2"></i>${currentUser.name}
                    </button>
                    <ul class="dropdown-menu">
                        <li><a class="dropdown-item" href="/dashboard"><i class="fas fa-tachometer-alt me-2"></i>Dashboard</a></li>
                        <li><a class="dropdown-item" href="/profile"><i class="fas fa-user-edit me-2"></i>Profil</a></li>
                        <li><a class="dropdown-item" href="/orders"><i class="fas fa-shopping-bag me-2"></i>Siparişlerim</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#" onclick="logout()"><i class="fas fa-sign-out-alt me-2"></i>Çıkış</a></li>
                    </ul>
                </div>
            `;
        }
    } else {
        // Show login/register buttons
        const userMenu = document.querySelector('.user-menu');
        if (userMenu) {
            userMenu.innerHTML = `
                <a href="/login" class="btn btn-outline-light me-2">Giriş Yap</a>
                <a href="/register" class="btn btn-light">Kayıt Ol</a>
            `;
        }
    }
}

// Update Cart Counter
function updateCartCounter() {
    const cartCounter = document.querySelector('.cart-counter');
    if (cartCounter) {
        cartCounter.textContent = cartItems.length;
        cartCounter.style.display = cartItems.length > 0 ? 'block' : 'none';
    }
}

// Update Wishlist Counter
function updateWishlistCounter() {
    const wishlistCounter = document.querySelector('.wishlist-counter');
    if (wishlistCounter) {
        wishlistCounter.textContent = wishlistItems.length;
        wishlistCounter.style.display = wishlistItems.length > 0 ? 'block' : 'none';
    }
}

// Show Notification
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

// Logout Function
function logout() {
    localStorage.removeItem('userToken');
    localStorage.removeItem('userData');
    currentUser = null;
    
    showNotification('Başarıyla çıkış yapıldı.', 'success');
    
    // Reload page to update UI
    setTimeout(() => {
        window.location.reload();
    }, 1000);
}

// Utility Functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function throttle(func, limit) {
    let inThrottle;
    return function() {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    };
}

// Performance Optimization
const optimizedScrollHandler = throttle(() => {
    // Handle scroll events efficiently
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    
    // Add scroll effects
    if (scrollTop > 100) {
        document.body.classList.add('scrolled');
    } else {
        document.body.classList.remove('scrolled');
    }
}, 16);

window.addEventListener('scroll', optimizedScrollHandler);

// Export functions for global use
window.marketplace = {
    showNotification,
    openCategoryPage,
    openStorePage,
    showProcessDetails,
    showTestimonialDetails,
    handleStoreApplication
};

