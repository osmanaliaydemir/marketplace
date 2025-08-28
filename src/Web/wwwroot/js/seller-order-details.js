/**
 * Seller Order Details JavaScript
 * Handles order details display and status updates
 */

class SellerOrderDetailsManager {
    constructor() {
        this.orderId = window.orderId;
        this.orderData = null;
        this.init();
    }

    init() {
        if (!this.orderId) {
            this.showError('Sipariş ID bulunamadı.');
            return;
        }
        
        this.bindEvents();
        this.loadOrderDetails();
    }

    bindEvents() {
        // Status update modal events
        document.getElementById('updateStatus')?.addEventListener('click', () => {
            this.updateOrderStatus();
        });

        // Date range change for custom dates
        document.getElementById('dateRange')?.addEventListener('change', (e) => {
            this.handleDateRangeChange(e.target.value);
        });
    }

    async loadOrderDetails() {
        try {
            this.showLoading();
            
            const response = await fetch(`/api/orders/${this.orderId}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            this.orderData = await response.json();
            this.renderOrderDetails();
            
        } catch (error) {
            console.error('Error loading order details:', error);
            this.showError('Sipariş detayları yüklenirken bir hata oluştu.');
        } finally {
            this.hideLoading();
        }
    }

    renderOrderDetails() {
        if (!this.orderData) return;

        // Order number
        document.getElementById('orderNumber')?.textContent = this.orderData.orderNumber || this.orderData.id;

        // Order summary
        document.getElementById('orderDate')?.textContent = this.formatDate(this.orderData.orderDate);
        document.getElementById('orderStatus')?.innerHTML = this.getStatusBadge(this.orderData.status);
        document.getElementById('paymentStatus')?.innerHTML = this.getPaymentBadge(this.orderData.paymentStatus);
        document.getElementById('totalAmount')?.textContent = `₺${this.orderData.totalAmount?.toFixed(2) || '0.00'}`;
        document.getElementById('shippingCost')?.textContent = `₺${this.orderData.shippingCost?.toFixed(2) || '0.00'}`;
        document.getElementById('taxAmount')?.textContent = `₺${this.orderData.taxAmount?.toFixed(2) || '0.00'}`;

        // Customer information
        document.getElementById('customerName')?.textContent = this.orderData.customerName || 'Bilinmiyor';
        document.getElementById('customerEmail')?.textContent = this.orderData.customerEmail || 'Bilinmiyor';
        document.getElementById('customerPhone')?.textContent = this.orderData.customerPhone || 'Bilinmiyor';
        document.getElementById('customerId')?.textContent = this.orderData.customerId || 'Bilinmiyor';

        // Addresses
        document.getElementById('shippingAddress')?.innerHTML = this.formatAddress(this.orderData.shippingAddress);
        document.getElementById('billingAddress')?.innerHTML = this.formatAddress(this.orderData.billingAddress);

        // Shipping information
        document.getElementById('shippingCompany')?.textContent = this.orderData.shippingCompany || 'Belirtilmemiş';
        document.getElementById('trackingNumber')?.textContent = this.orderData.trackingNumber || 'Belirtilmemiş';
        document.getElementById('shippingStatus')?.textContent = this.getShippingStatusText(this.orderData.shippingStatus);
        document.getElementById('estimatedDelivery')?.textContent = this.formatDate(this.orderData.estimatedDelivery);

        // Order items
        this.renderOrderItems();

        // Order history
        this.renderOrderHistory();

        // Set current status in modal
        if (document.getElementById('newStatus')) {
            document.getElementById('newStatus').value = this.orderData.status || 'Pending';
        }
    }

    renderOrderItems() {
        const tbody = document.getElementById('orderItemsTable');
        if (!tbody || !this.orderData.orderItems) return;

        if (this.orderData.orderItems.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="4" class="text-center py-3">
                        <em class="text-muted">Sipariş öğesi bulunamadı.</em>
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = this.orderData.orderItems.map(item => `
            <tr>
                <td>
                    <div class="d-flex align-items-center">
                        ${item.imageUrl ? `<img src="${item.imageUrl}" alt="${item.productName}" class="me-3" style="width: 50px; height: 50px; object-fit: cover;">` : ''}
                        <div>
                            <strong>${item.productName}</strong>
                            ${item.sku ? `<br><small class="text-muted">SKU: ${item.sku}</small>` : ''}
                        </div>
                    </div>
                </td>
                <td>₺${item.unitPrice?.toFixed(2) || '0.00'}</td>
                <td>${item.quantity || 0}</td>
                <td><strong>₺${((item.unitPrice || 0) * (item.quantity || 0)).toFixed(2)}</strong></td>
            </tr>
        `).join('');
    }

    renderOrderHistory() {
        const container = document.getElementById('orderHistory');
        if (!container || !this.orderData.orderHistory) return;

        if (this.orderData.orderHistory.length === 0) {
            container.innerHTML = '<em class="text-muted">Sipariş geçmişi bulunamadı.</em>';
            return;
        }

        container.innerHTML = this.orderData.orderHistory.map(history => `
            <div class="d-flex align-items-start mb-3">
                <div class="flex-shrink-0">
                    <div class="bg-light rounded-circle d-flex align-items-center justify-content-center" 
                         style="width: 32px; height: 32px;">
                        <i class="bi bi-clock text-muted"></i>
                    </div>
                </div>
                <div class="flex-grow-1 ms-3">
                    <div class="d-flex justify-content-between align-items-start">
                        <div>
                            <strong>${this.getStatusText(history.status)}</strong>
                            ${history.note ? `<br><small class="text-muted">${history.note}</small>` : ''}
                        </div>
                        <small class="text-muted">${this.formatDateTime(history.timestamp)}</small>
                    </div>
                </div>
            </div>
        `).join('');
    }

    async updateOrderStatus() {
        const newStatus = document.getElementById('newStatus')?.value;
        const note = document.getElementById('statusNote')?.value;
        const trackingNumber = document.getElementById('trackingNumberInput')?.value;
        const sendNotification = document.getElementById('sendNotification')?.checked;

        if (!newStatus) {
            this.showError('Lütfen yeni durumu seçin.');
            return;
        }

        try {
            const response = await fetch(`/api/orders/${this.orderId}/status`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    status: newStatus,
                    note: note,
                    trackingNumber: trackingNumber,
                    sendNotification: sendNotification
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('statusUpdateModal'));
            modal.hide();
            
            // Refresh order details
            await this.loadOrderDetails();
            
            // Show success message
            this.showSuccess('Sipariş durumu başarıyla güncellendi.');
            
        } catch (error) {
            console.error('Error updating order status:', error);
            this.showError('Sipariş durumu güncellenirken bir hata oluştu.');
        }
    }

    handleDateRangeChange(value) {
        const customInputs = document.getElementById('customDateInputs');
        const customInputs2 = document.getElementById('customDateInputs2');
        
        if (value === 'custom') {
            customInputs.style.display = 'block';
            customInputs2.style.display = 'block';
        } else {
            customInputs.style.display = 'none';
            customInputs2.style.display = 'none';
        }
    }

    // Utility methods
    formatDate(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR');
    }

    formatDateTime(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleString('tr-TR');
    }

    formatAddress(address) {
        if (!address) return 'Adres bilgisi bulunamadı.';
        
        return `
            <strong>${address.fullName || ''}</strong><br>
            ${address.addressLine1 || ''}<br>
            ${address.addressLine2 ? address.addressLine2 + '<br>' : ''}
            ${address.city || ''}, ${address.state || ''} ${address.postalCode || ''}<br>
            ${address.country || ''}
        `;
    }

    getStatusBadge(status) {
        const classes = {
            'Pending': 'bg-warning',
            'Processing': 'bg-info',
            'Shipped': 'bg-primary',
            'Delivered': 'bg-success',
            'Cancelled': 'bg-danger'
        };
        
        const texts = {
            'Pending': 'Beklemede',
            'Processing': 'Hazırlanıyor',
            'Shipped': 'Kargoda',
            'Delivered': 'Teslim Edildi',
            'Cancelled': 'İptal Edildi'
        };
        
        const badgeClass = classes[status] || 'bg-secondary';
        const badgeText = texts[status] || status;
        
        return `<span class="badge ${badgeClass}">${badgeText}</span>`;
    }

    getPaymentBadge(status) {
        const classes = {
            'Pending': 'bg-warning',
            'Paid': 'bg-success',
            'Failed': 'bg-danger',
            'Refunded': 'bg-info'
        };
        
        const texts = {
            'Pending': 'Beklemede',
            'Paid': 'Ödendi',
            'Failed': 'Başarısız',
            'Refunded': 'İade Edildi'
        };
        
        const badgeClass = classes[status] || 'bg-secondary';
        const badgeText = texts[status] || status;
        
        return `<span class="badge ${badgeClass}">${badgeText}</span>`;
    }

    getStatusText(status) {
        const texts = {
            'Pending': 'Beklemede',
            'Processing': 'Hazırlanıyor',
            'Shipped': 'Kargoda',
            'Delivered': 'Teslim Edildi',
            'Cancelled': 'İptal Edildi'
        };
        return texts[status] || status;
    }

    getShippingStatusText(status) {
        const texts = {
            'Pending': 'Beklemede',
            'InTransit': 'Yolda',
            'OutForDelivery': 'Dağıtıma Çıktı',
            'Delivered': 'Teslim Edildi',
            'Failed': 'Teslim Edilemedi'
        };
        return texts[status] || status || 'Belirtilmemiş';
    }

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    showLoading() {
        // Add loading indicator to main content
        const mainContent = document.querySelector('section.container');
        if (mainContent) {
            const loadingDiv = document.createElement('div');
            loadingDiv.id = 'loadingIndicator';
            loadingDiv.className = 'text-center py-5';
            loadingDiv.innerHTML = `
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
                <p class="mt-2 text-muted">Sipariş detayları yükleniyor...</p>
            `;
            mainContent.appendChild(loadingDiv);
        }
    }

    hideLoading() {
        const loadingIndicator = document.getElementById('loadingIndicator');
        if (loadingIndicator) {
            loadingIndicator.remove();
        }
    }

    showSuccess(message) {
        // You can implement a toast notification system here
        alert(message);
    }

    showError(message) {
        // You can implement a toast notification system here
        alert('Hata: ' + message);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.orderDetailsManager = new SellerOrderDetailsManager();
});
