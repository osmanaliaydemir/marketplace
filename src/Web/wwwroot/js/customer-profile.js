/**
 * Customer Profile Manager
 * Handles profile management, address management, and account settings
 */
class CustomerProfileManager {
    constructor() {
        this.isEditing = false;
        this.addresses = [];
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadProfileData();
    }

    bindEvents() {
        // Profile form submission
        document.getElementById('profileForm')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveProfile();
        });

        // Address form submission
        document.getElementById('addressForm')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveAddress();
        });

        // Change password form submission
        document.getElementById('changePasswordForm')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.savePassword();
        });
    }

    async loadProfileData() {
        try {
            this.showLoading(true);
            
            // Load profile and addresses in parallel
            const [profileData, addressesData] = await Promise.all([
                this.fetchProfileData(),
                this.fetchAddressesData()
            ]);

            this.populateProfileForm(profileData);
            this.populateAddressesList(addressesData);
            this.updateAccountSummary(profileData);
            
            this.showProfile();
            
        } catch (error) {
            console.error('Error loading profile data:', error);
            this.showError('Profil bilgileri yüklenirken bir hata oluştu: ' + error.message);
        } finally {
            this.showLoading(false);
        }
    }

    async fetchProfileData() {
        try {
            const response = await fetch('?handler=GetProfile', {
                method: 'POST',
                headers: { 
                    'RequestVerificationToken': this.getAntiForgeryToken(),
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                return await response.json();
            }
            
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        } catch (error) {
            console.error('Error fetching profile:', error);
            throw error;
        }
    }

    async fetchAddressesData() {
        try {
            const response = await fetch('?handler=GetAddresses', {
                method: 'POST',
                headers: { 
                    'RequestVerificationToken': this.getAntiForgeryToken(),
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                return await response.json();
            }
            
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        } catch (error) {
            console.error('Error fetching addresses:', error);
            throw error;
        }
    }

    populateProfileForm(profileData) {
        document.getElementById('firstName').value = profileData.firstName || '';
        document.getElementById('lastName').value = profileData.lastName || '';
        document.getElementById('email').value = profileData.email || '';
        document.getElementById('phone').value = profileData.phone || '';
        document.getElementById('birthDate').value = profileData.birthDate || '';
        document.getElementById('gender').value = profileData.gender || '';
    }

    populateAddressesList(addressesData) {
        this.addresses = addressesData || [];
        const addressesList = document.getElementById('addressesList');
        
        if (!addressesList) return;

        if (this.addresses.length === 0) {
            addressesList.innerHTML = `
                <div class="text-center text-muted py-4">
                    <i class="fas fa-map-marker-alt fa-2x mb-2"></i>
                    <br>Henüz adres eklenmemiş
                </div>
            `;
            return;
        }

        addressesList.innerHTML = this.addresses.map((address, index) => `
            <div class="border rounded p-3 mb-3">
                <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                        <div class="d-flex align-items-center mb-2">
                            <h6 class="mb-0 me-2">${address.title}</h6>
                            ${address.isDefault ? '<span class="badge bg-primary">Varsayılan</span>' : ''}
                        </div>
                        <p class="mb-1"><strong>${address.recipientName}</strong></p>
                        <p class="mb-1 text-muted">${address.addressLine1}</p>
                        ${address.addressLine2 ? `<p class="mb-1 text-muted">${address.addressLine2}</p>` : ''}
                        <p class="mb-1 text-muted">${address.city}, ${address.state} ${address.postalCode}</p>
                        <p class="mb-0 text-muted">📞 ${address.phone}</p>
                    </div>
                    <div class="d-flex gap-1">
                        <button type="button" class="btn btn-sm btn-outline-primary" onclick="profileManager.editAddress(${index})">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="profileManager.deleteAddress(${address.id})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `).join('');
    }

    updateAccountSummary(profileData) {
        document.getElementById('memberSince').textContent = this.formatDate(profileData.memberSince);
        document.getElementById('totalOrders').textContent = profileData.totalOrders || 0;
        document.getElementById('totalWishlist').textContent = profileData.totalWishlist || 0;
        document.getElementById('lastLogin').textContent = this.formatDate(profileData.lastLogin);
    }

    editProfile() {
        this.isEditing = true;
        this.enableFormEditing();
        document.getElementById('saveProfileBtn').style.display = 'inline-block';
        document.getElementById('cancelEditBtn').style.display = 'inline-block';
        document.querySelector('.btn-outline-primary[onclick="profileManager.editProfile()"]').style.display = 'none';
    }

    cancelEdit() {
        this.isEditing = false;
        this.disableFormEditing();
        this.loadProfileData(); // Reload original data
    }

    enableFormEditing() {
        const form = document.getElementById('profileForm');
        const inputs = form.querySelectorAll('input, select');
        inputs.forEach(input => {
            if (input.id !== 'email') { // Email should remain readonly
                input.removeAttribute('readonly');
                input.disabled = false;
            }
        });
    }

    disableFormEditing() {
        const form = document.getElementById('profileForm');
        const inputs = form.querySelectorAll('input, select');
        inputs.forEach(input => {
            if (input.id !== 'email') {
                input.setAttribute('readonly', true);
                input.disabled = true;
            }
        });
        document.getElementById('saveProfileBtn').style.display = 'none';
        document.getElementById('cancelEditBtn').style.display = 'none';
        document.querySelector('.btn-outline-primary[onclick="profileManager.editProfile()"]').style.display = 'inline-block';
    }

    async saveProfile() {
        try {
            const formData = this.getFormData();
            
            if (!this.validateProfileData(formData)) {
                return;
            }

            const response = await fetch('?handler=UpdateProfile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                this.showSuccess('Profil bilgileri başarıyla güncellendi');
                this.cancelEdit();
                this.loadProfileData();
            } else {
                const error = await response.json();
                this.showError(error.message || 'Profil güncellenirken bir hata oluştu');
            }
        } catch (error) {
            console.error('Error saving profile:', error);
            this.showError('Profil güncellenirken bir hata oluştu');
        }
    }

    getFormData() {
        return {
            firstName: document.getElementById('firstName').value,
            lastName: document.getElementById('lastName').value,
            phone: document.getElementById('phone').value,
            birthDate: document.getElementById('birthDate').value,
            gender: document.getElementById('gender').value
        };
    }

    validateProfileData(data) {
        if (!data.firstName.trim()) {
            this.showError('Ad alanı zorunludur');
            return false;
        }
        if (!data.lastName.trim()) {
            this.showError('Soyad alanı zorunludur');
            return false;
        }
        if (data.phone && !this.isValidPhone(data.phone)) {
            this.showError('Geçerli bir telefon numarası giriniz');
            return false;
        }
        return true;
    }

    addAddress() {
        this.resetAddressForm();
        document.getElementById('addressModalTitle').textContent = 'Yeni Adres Ekle';
        document.getElementById('addressId').value = '';
        const modal = new bootstrap.Modal(document.getElementById('addressModal'));
        modal.show();
    }

    editAddress(index) {
        const address = this.addresses[index];
        this.populateAddressForm(address);
        document.getElementById('addressModalTitle').textContent = 'Adres Düzenle';
        document.getElementById('addressId').value = address.id;
        const modal = new bootstrap.Modal(document.getElementById('addressModal'));
        modal.show();
    }

    populateAddressForm(address) {
        document.getElementById('addressTitle').value = address.title || '';
        document.getElementById('recipientName').value = address.recipientName || '';
        document.getElementById('addressLine1').value = address.addressLine1 || '';
        document.getElementById('addressLine2').value = address.addressLine2 || '';
        document.getElementById('city').value = address.city || '';
        document.getElementById('state').value = address.state || '';
        document.getElementById('postalCode').value = address.postalCode || '';
        document.getElementById('addressPhone').value = address.phone || '';
        document.getElementById('isDefault').checked = address.isDefault || false;
    }

    resetAddressForm() {
        document.getElementById('addressForm').reset();
        document.getElementById('addressId').value = '';
    }

    async saveAddress() {
        try {
            const formData = this.getAddressFormData();
            
            if (!this.validateAddressData(formData)) {
                return;
            }

            const addressId = document.getElementById('addressId').value;
            const handler = addressId ? 'UpdateAddress' : 'SaveAddress';
            const url = `?handler=${handler}`;

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                this.showSuccess(addressId ? 'Adres başarıyla güncellendi' : 'Adres başarıyla eklendi');
                const modal = bootstrap.Modal.getInstance(document.getElementById('addressModal'));
                modal.hide();
                this.loadProfileData();
            } else {
                const error = await response.json();
                this.showError(error.message || 'Adres kaydedilirken bir hata oluştu');
            }
        } catch (error) {
            console.error('Error saving address:', error);
            this.showError('Adres kaydedilirken bir hata oluştu');
        }
    }

    getAddressFormData() {
        return {
            title: document.getElementById('addressTitle').value,
            recipientName: document.getElementById('recipientName').value,
            addressLine1: document.getElementById('addressLine1').value,
            addressLine2: document.getElementById('addressLine2').value,
            city: document.getElementById('city').value,
            state: document.getElementById('state').value,
            postalCode: document.getElementById('postalCode').value,
            phone: document.getElementById('addressPhone').value,
            isDefault: document.getElementById('isDefault').checked
        };
    }

    validateAddressData(data) {
        if (!data.title.trim()) {
            this.showError('Adres başlığı zorunludur');
            return false;
        }
        if (!data.recipientName.trim()) {
            this.showError('Alıcı adı zorunludur');
            return false;
        }
        if (!data.addressLine1.trim()) {
            this.showError('Adres satırı zorunludur');
            return false;
        }
        if (!data.city.trim()) {
            this.showError('Şehir zorunludur');
            return false;
        }
        if (!data.state.trim()) {
            this.showError('İlçe zorunludur');
            return false;
        }
        if (!data.phone.trim()) {
            this.showError('Telefon zorunludur');
            return false;
        }
        if (!this.isValidPhone(data.phone)) {
            this.showError('Geçerli bir telefon numarası giriniz');
            return false;
        }
        return true;
    }

    async deleteAddress(addressId) {
        if (!confirm('Bu adresi silmek istediğinizden emin misiniz?')) {
            return;
        }

        try {
            const response = await fetch(`?handler=DeleteAddress&id=${addressId}`, {
                method: 'POST',
                headers: { 
                    'RequestVerificationToken': this.getAntiForgeryToken() 
                }
            });

            if (response.ok) {
                this.showSuccess('Adres başarıyla silindi');
                this.loadProfileData();
            } else {
                const error = await response.json();
                this.showError(error.message || 'Adres silinirken bir hata oluştu');
            }
        } catch (error) {
            console.error('Error deleting address:', error);
            this.showError('Adres silinirken bir hata oluştu');
        }
    }

    changePassword() {
        document.getElementById('changePasswordForm').reset();
        const modal = new bootstrap.Modal(document.getElementById('changePasswordModal'));
        modal.show();
    }

    async savePassword() {
        try {
            const formData = this.getPasswordFormData();
            
            if (!this.validatePasswordData(formData)) {
                return;
            }

            const response = await fetch('?handler=ChangePassword', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                this.showSuccess('Şifre başarıyla değiştirildi');
                const modal = bootstrap.Modal.getInstance(document.getElementById('changePasswordModal'));
                modal.hide();
            } else {
                const error = await response.json();
                this.showError(error.message || 'Şifre değiştirilirken bir hata oluştu');
            }
        } catch (error) {
            console.error('Error changing password:', error);
            this.showError('Şifre değiştirilirken bir hata oluştu');
        }
    }

    getPasswordFormData() {
        return {
            currentPassword: document.getElementById('currentPassword').value,
            newPassword: document.getElementById('newPassword').value,
            confirmPassword: document.getElementById('confirmPassword').value
        };
    }

    validatePasswordData(data) {
        if (!data.currentPassword) {
            this.showError('Mevcut şifre zorunludur');
            return false;
        }
        if (!data.newPassword) {
            this.showError('Yeni şifre zorunludur');
            return false;
        }
        if (data.newPassword.length < 8) {
            this.showError('Yeni şifre en az 8 karakter olmalıdır');
            return false;
        }
        if (data.newPassword !== data.confirmPassword) {
            this.showError('Yeni şifreler eşleşmiyor');
            return false;
        }
        return true;
    }

    deleteAccount() {
        if (!confirm('Hesabınızı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!')) {
            return;
        }

        if (!confirm('Tüm verileriniz kalıcı olarak silinecektir. Devam etmek istiyor musunuz?')) {
            return;
        }

        // Account deletion logic would go here
        this.showError('Hesap silme özelliği henüz aktif değil');
    }

    // Utility methods
    isValidPhone(phone) {
        const phoneRegex = /^(\+90|0)?[5][0-9]{9}$/;
        return phoneRegex.test(phone.replace(/\s/g, ''));
    }

    formatDate(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    }

    showLoading(show) {
        const spinner = document.getElementById('loadingSpinner');
        const content = document.getElementById('profileContent');
        
        if (show) {
            spinner.style.display = 'block';
            content.style.display = 'none';
        } else {
            spinner.style.display = 'none';
            content.style.display = 'block';
        }
    }

    showProfile() {
        document.getElementById('profileContent').style.display = 'block';
    }

    showSuccess(message) {
        this.showNotification(message, 'success');
    }

    showError(message) {
        this.showNotification(message, 'danger');
    }

    showNotification(message, type) {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(notification);
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            notification.remove();
        }, 5000);
    }

    // Anti-forgery token alma metodu
    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.profileManager = new CustomerProfileManager();
});
