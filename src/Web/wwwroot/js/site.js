// Marketplace Site JavaScript

// Utility Functions
const Marketplace = {
    // Show alert message
    showAlert: function(message, type = 'info') {
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        
        // Insert at the top of the main content
        const mainContent = document.querySelector('main') || document.body;
        mainContent.insertAdjacentHTML('afterbegin', alertHtml);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const alert = document.querySelector('.alert');
            if (alert) {
                alert.remove();
            }
        }, 5000);
    },

    // Format price
    formatPrice: function(price, currency = 'TRY') {
        return new Intl.NumberFormat('tr-TR', {
            style: 'currency',
            currency: currency
        }).format(price);
    },

    // Validate email
    validateEmail: function(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    },

    // Show loading spinner
    showLoading: function() {
        const spinner = document.createElement('div');
        spinner.id = 'loading-spinner';
        spinner.className = 'loading-spinner';
        spinner.innerHTML = `
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Yükleniyor...</span>
            </div>
        `;
        document.body.appendChild(spinner);
    },

    // Hide loading spinner
    hideLoading: function() {
        const spinner = document.getElementById('loading-spinner');
        if (spinner) {
            spinner.remove();
        }
    },

    // Handle form submission
    handleFormSubmit: function(form, callback) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            
            Marketplace.showLoading();
            
            const formData = new FormData(form);
            const data = Object.fromEntries(formData.entries());
            
            callback(data, form);
        });
    },

    // Add to cart
    addToCart: function(productId, quantity = 1) {
        Marketplace.showLoading();
        
        fetch('/api/cart/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                productId: productId,
                quantity: quantity
            })
        })
        .then(response => response.json())
        .then(data => {
            Marketplace.hideLoading();
            
            if (data.success) {
                Marketplace.showAlert('Ürün sepete eklendi!', 'success');
                // Update cart count
                this.updateCartCount();
            } else {
                Marketplace.showAlert(data.message || 'Bir hata oluştu!', 'danger');
            }
        })
        .catch(error => {
            Marketplace.hideLoading();
            Marketplace.showAlert('Bir hata oluştu!', 'danger');
            console.error('Error:', error);
        });
    },

    // Update cart count
    updateCartCount: function() {
        fetch('/api/cart/count')
        .then(response => response.json())
        .then(data => {
            const cartCount = document.querySelector('.cart-count');
            if (cartCount) {
                cartCount.textContent = data.count;
            }
        })
        .catch(error => {
            console.error('Error updating cart count:', error);
        });
    },

    // Search products
    searchProducts: function(query) {
        if (query.length < 2) return;
        
        Marketplace.showLoading();
        
        fetch(`/api/products/search?q=${encodeURIComponent(query)}`)
        .then(response => response.json())
        .then(data => {
            Marketplace.hideLoading();
            
            const resultsContainer = document.getElementById('search-results');
            if (resultsContainer) {
                resultsContainer.innerHTML = '';
                
                if (data.products && data.products.length > 0) {
                    data.products.forEach(product => {
                        const productHtml = `
                            <div class="search-result-item">
                                <img src="${product.imageUrl || '/images/placeholder.jpg'}" alt="${product.name}">
                                <div>
                                    <h6>${product.name}</h6>
                                    <p>${Marketplace.formatPrice(product.price)}</p>
                                </div>
                            </div>
                        `;
                        resultsContainer.insertAdjacentHTML('beforeend', productHtml);
                    });
                } else {
                    resultsContainer.innerHTML = '<p>Ürün bulunamadı.</p>';
                }
            }
        })
        .catch(error => {
            Marketplace.hideLoading();
            console.error('Error searching products:', error);
        });
    },

    // Initialize tooltips
    initTooltips: function() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    },

    // Initialize popovers
    initPopovers: function() {
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }
};

// Document ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips and popovers
    Marketplace.initTooltips();
    Marketplace.initPopovers();
    
    // Handle search input
    const searchInput = document.getElementById('search-input');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                Marketplace.searchProducts(this.value);
            }, 300);
        });
    }
    
    // Handle add to cart buttons
    const addToCartButtons = document.querySelectorAll('.add-to-cart-btn');
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const productId = this.dataset.productId;
            const quantity = parseInt(this.dataset.quantity) || 1;
            Marketplace.addToCart(productId, quantity);
        });
    });
    
    // Handle quantity inputs
    const quantityInputs = document.querySelectorAll('.quantity-input');
    quantityInputs.forEach(input => {
        input.addEventListener('change', function() {
            const addToCartBtn = this.closest('.product-actions').querySelector('.add-to-cart-btn');
            if (addToCartBtn) {
                addToCartBtn.dataset.quantity = this.value;
            }
        });
    });
    
    // Auto-dismiss alerts
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert && alert.parentNode) {
                alert.remove();
            }
        }, 5000);
    });
});

// Add loading spinner styles
const spinnerStyles = `
    .loading-spinner {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(255, 255, 255, 0.8);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 9999;
    }
    
    .search-result-item {
        display: flex;
        align-items: center;
        padding: 10px;
        border-bottom: 1px solid #eee;
    }
    
    .search-result-item img {
        width: 50px;
        height: 50px;
        object-fit: cover;
        margin-right: 10px;
    }
    
    .search-result-item h6 {
        margin: 0;
        font-size: 14px;
    }
    
    .search-result-item p {
        margin: 0;
        font-size: 12px;
        color: #666;
    }
`;

// Inject styles
const styleSheet = document.createElement('style');
styleSheet.textContent = spinnerStyles;
document.head.appendChild(styleSheet);

