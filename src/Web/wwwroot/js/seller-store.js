/**
 * Seller Store Management
 * Handles store information loading, editing, and updating
 */
class SellerStoreManager {
    constructor() {
        this.storeData = null;
        this.originalData = null;
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadStoreData();
    }

    bindEvents() {
        // Form submission
        document.getElementById('storeEditForm')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveStoreData();
        });

        // Slug auto-generation from name
        document.getElementById('storeName')?.addEventListener('input', (e) => {
            this.generateSlug(e.target.value);
        });
    }

    async loadStoreData() {
        try {
            this.showLoading(true);
            this.hideError();

            const response = await fetch('/api/stores/mine', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }

            this.storeData = await response.json();
            this.originalData = JSON.parse(JSON.stringify(this.storeData));
            
            this.populateForm();
            this.showStoreForm();
            this.loadStoreStats();
            
        } catch (error) {
            console.error('Error loading store data:', error);
            this.showError('Mağaza bilgileri yüklenirken bir hata oluştu: ' + error.message);
        } finally {
            this.showLoading(false);
        }
    }

    populateForm() {
        if (!this.storeData) return;

        // Basic Information
        document.getElementById('storeName').value = this.storeData.name || '';
        document.getElementById('storeSlug').value = this.storeData.slug || '';
        document.getElementById('storeDescription').value = this.storeData.description || '';

        // Contact Information
        document.getElementById('storePhone').value = this.storeData.phone || '';
        document.getElementById('storeEmail').value = this.storeData.email || '';
        document.getElementById('storeWebsite').value = this.storeData.website || '';
        document.getElementById('storeAddress').value = this.storeData.address || '';

        // Settings
        document.getElementById('storeCurrency').value = this.storeData.currency || 'TRY';
        document.getElementById('storeLanguage').value = this.storeData.language || 'tr';
        document.getElementById('storeIsActive').checked = this.storeData.isActive !== false;
    }

    async saveStoreData() {
        try {
            this.showLoading(true);
            this.hideError();

            const formData = this.getFormData();
            
            // Validation
            if (!this.validateFormData(formData)) {
                return;
            }

            const response = await fetch('/api/stores/mine', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(formData)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || `HTTP ${response.status}`);
            }

            const updatedStore = await response.json();
            this.storeData = updatedStore;
            this.originalData = JSON.parse(JSON.stringify(updatedStore));
            
            this.showSuccess('Mağaza bilgileri başarıyla güncellendi!');
            
        } catch (error) {
            console.error('Error saving store data:', error);
            this.showError('Mağaza bilgileri kaydedilirken bir hata oluştu: ' + error.message);
        } finally {
            this.showLoading(false);
        }
    }

    getFormData() {
        return {
            name: document.getElementById('storeName').value.trim(),
            slug: document.getElementById('storeSlug').value.trim(),
            description: document.getElementById('storeDescription').value.trim(),
            phone: document.getElementById('storePhone').value.trim(),
            email: document.getElementById('storeEmail').value.trim(),
            website: document.getElementById('storeWebsite').value.trim(),
            address: document.getElementById('storeAddress').value.trim(),
            currency: document.getElementById('storeCurrency').value,
            language: document.getElementById('storeLanguage').value,
            isActive: document.getElementById('storeIsActive').checked
        };
    }

    validateFormData(data) {
        if (!data.name) {
            this.showError('Mağaza adı zorunludur');
            return false;
        }

        if (!data.slug) {
            this.showError('Mağaza URL\'i zorunludur');
            return false;
        }

        if (data.slug.length < 3) {
            this.showError('Mağaza URL\'i en az 3 karakter olmalıdır');
            return false;
        }

        if (!/^[a-z0-9-]+$/.test(data.slug)) {
            this.showError('Mağaza URL\'i sadece küçük harf, rakam ve tire içerebilir');
            return false;
        }

        if (data.email && !this.isValidEmail(data.email)) {
            this.showError('Geçerli bir e-posta adresi giriniz');
            return false;
        }

        if (data.website && !this.isValidUrl(data.website)) {
            this.showError('Geçerli bir web sitesi URL\'i giriniz');
            return false;
        }

        return true;
    }

    generateSlug(name) {
        if (!name) return;
        
        const slug = name
            .toLowerCase()
            .replace(/ğ/g, 'g')
            .replace(/ü/g, 'u')
            .replace(/ş/g, 's')
            .replace(/ı/g, 'i')
            .replace(/ö/g, 'o')
            .replace(/ç/g, 'c')
            .replace(/[^a-z0-9\s-]/g, '')
            .replace(/\s+/g, '-')
            .replace(/-+/g, '-')
            .trim('-');
        
        document.getElementById('storeSlug').value = slug;
    }

    resetForm() {
        if (this.originalData) {
            this.storeData = JSON.parse(JSON.stringify(this.originalData));
            this.populateForm();
        }
    }

    async loadStoreStats() {
        try {
            // Load product count
            const productsResponse = await fetch('/api/products/mine', {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (productsResponse.ok) {
                const productsData = await productsResponse.json();
                document.getElementById('totalProducts').textContent = productsData.totalCount || 0;
                document.getElementById('activeProducts').textContent = 
                    productsData.items?.filter(p => p.isActive).length || 0;
            }

            // Load order count
            const ordersResponse = await fetch('/api/orders/seller', {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (ordersResponse.ok) {
                const ordersData = await ordersResponse.json();
                document.getElementById('totalOrders').textContent = ordersData.totalCount || 0;
                document.getElementById('pendingOrders').textContent = 
                    ordersData.items?.filter(o => o.status === 'Pending').length || 0;
            }

            this.showStoreStats();
            
        } catch (error) {
            console.error('Error loading store stats:', error);
            // Don't show error for stats, just hide the section
            this.hideStoreStats();
        }
    }

    // Utility methods
    showLoading(show) {
        const spinner = document.getElementById('loadingSpinner');
        const form = document.getElementById('storeForm');
        
        if (show) {
            spinner.style.display = 'block';
            form.style.display = 'none';
        } else {
            spinner.style.display = 'none';
            form.style.display = 'block';
        }
    }

    showStoreForm() {
        document.getElementById('storeForm').style.display = 'block';
    }

    showStoreStats() {
        document.getElementById('storeStats').style.display = 'block';
    }

    hideStoreStats() {
        document.getElementById('storeStats').style.display = 'none';
    }

    showError(message) {
        const errorDiv = document.getElementById('errorMessage');
        const errorText = document.getElementById('errorText');
        
        errorText.textContent = message;
        errorDiv.style.display = 'block';
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            errorDiv.style.display = 'none';
        }, 5000);
    }

    hideError() {
        document.getElementById('errorMessage').style.display = 'none';
    }

    showSuccess(message) {
        // Create temporary success message
        const successDiv = document.createElement('div');
        successDiv.className = 'alert alert-success alert-dismissible fade show';
        successDiv.innerHTML = `
            <i class="fas fa-check-circle me-2"></i>${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        const container = document.querySelector('.container-fluid');
        container.insertBefore(successDiv, container.firstChild);
        
        // Auto-hide after 3 seconds
        setTimeout(() => {
            successDiv.remove();
        }, 3000);
    }

    isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    isValidUrl(url) {
        try {
            new URL(url);
            return true;
        } catch {
            return false;
        }
    }

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.storeManager = new SellerStoreManager();
});
