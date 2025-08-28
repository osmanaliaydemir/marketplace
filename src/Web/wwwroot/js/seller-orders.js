/**
 * Seller Orders Management JavaScript
 * Handles orders listing, filtering, pagination, and bulk operations
 */

class SellerOrdersManager {
    constructor() {
        this.currentPage = 1;
        this.pageSize = 20;
        this.filters = {
            status: '',
            startDate: '',
            endDate: '',
            searchTerm: '',
            minAmount: '',
            maxAmount: '',
            paymentMethod: '',
            shippingCompany: ''
        };
        this.selectedOrders = new Set();
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadOrders();
        this.setupDateDefaults();
    }

    bindEvents() {
        // Filter events
        document.getElementById('statusFilter')?.addEventListener('change', (e) => {
            this.filters.status = e.target.value;
            this.currentPage = 1;
            this.loadOrders();
        });

        document.getElementById('startDate')?.addEventListener('change', (e) => {
            this.filters.startDate = e.target.value;
            this.currentPage = 1;
            this.loadOrders();
        });

        document.getElementById('endDate')?.addEventListener('change', (e) => {
            this.filters.endDate = e.target.value;
            this.currentPage = 1;
            this.loadOrders();
        });

        document.getElementById('searchInput')?.addEventListener('input', this.debounce((e) => {
            this.filters.searchTerm = e.target.value;
            this.currentPage = 1;
            this.loadOrders();
        }, 500));

        // Bulk selection
        document.getElementById('selectAll')?.addEventListener('change', (e) => {
            this.toggleSelectAll(e.target.checked);
        });

        // Refresh button
        document.getElementById('refreshBtn')?.addEventListener('click', () => {
            this.loadOrders();
        });

        // Bulk actions
        document.getElementById('bulkStatusBtn')?.addEventListener('click', () => {
            this.showBulkStatusUpdate();
        });

        document.getElementById('bulkExportBtn')?.addEventListener('click', () => {
            this.exportSelectedOrders();
        });

        // Modal events
        document.getElementById('applyFilters')?.addEventListener('click', () => {
            this.applyAdvancedFilters();
        });

        document.getElementById('updateStatus')?.addEventListener('click', () => {
            this.updateOrderStatus();
        });
    }

    setupDateDefaults() {
        const today = new Date();
        const thirtyDaysAgo = new Date(today.getTime() - (30 * 24 * 60 * 60 * 1000));
        
        if (document.getElementById('startDate')) {
            document.getElementById('startDate').value = this.formatDate(thirtyDaysAgo);
            this.filters.startDate = this.formatDate(thirtyDaysAgo);
        }
        
        if (document.getElementById('endDate')) {
            document.getElementById('endDate').value = this.formatDate(today);
            this.filters.endDate = this.formatDate(today);
        }
    }

    async loadOrders() {
        try {
            this.showLoading();
            
            const queryParams = new URLSearchParams({
                page: this.currentPage,
                pageSize: this.pageSize,
                ...this.filters
            });

            const response = await fetch(`/api/orders/seller?${queryParams}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            this.renderOrders(data.orders || []);
            this.renderPagination(data.totalCount || 0);
            this.updateBulkActions();
            
        } catch (error) {
            console.error('Error loading orders:', error);
            this.showError('Siparişler yüklenirken bir hata oluştu.');
        } finally {
            this.hideLoading();
        }
    }

    renderOrders(orders) {
        const tbody = document.getElementById('ordersTableBody');
        if (!tbody) return;

        if (orders.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-4">
                        <div class="text-muted">
                            <i class="bi bi-inbox fs-1 d-block mb-2"></i>
                            <p class="mb-0">Henüz sipariş bulunmuyor.</p>
                        </div>
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = orders.map(order => `
            <tr data-order-id="${order.id}">
                <td>
                    <input type="checkbox" class="form-check-input order-checkbox" 
                           value="${order.id}" ${this.selectedOrders.has(order.id) ? 'checked' : ''}>
                </td>
                <td>
                    <strong>#${order.orderNumber}</strong>
                    <br><small class="text-muted">${order.id}</small>
                </td>
                <td>${this.formatDate(order.orderDate)}</td>
                <td>
                    <div>
                        <strong>${order.customerName}</strong>
                        <br><small class="text-muted">${order.customerEmail}</small>
                    </div>
                </td>
                <td>
                    <strong class="text-primary">₺${order.totalAmount.toFixed(2)}</strong>
                    <br><small class="text-muted">${order.currency}</small>
                </td>
                <td>
                    <span class="badge ${this.getStatusBadgeClass(order.status)}">
                        ${this.getStatusText(order.status)}
                    </span>
                </td>
                <td>
                    <span class="badge ${this.getPaymentBadgeClass(order.paymentStatus)}">
                        ${this.getPaymentText(order.paymentStatus)}
                    </span>
                </td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <a href="/satici/siparisler/${order.id}" class="btn btn-outline-primary btn-sm">
                            <i class="bi bi-eye"></i>
                        </a>
                        <button type="button" class="btn btn-outline-secondary btn-sm" 
                                onclick="ordersManager.updateSingleOrderStatus(${order.id})">
                            <i class="bi bi-gear"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');

        // Bind checkbox events
        tbody.querySelectorAll('.order-checkbox').forEach(checkbox => {
            checkbox.addEventListener('change', (e) => {
                this.toggleOrderSelection(e.target.value, e.target.checked);
            });
        });
    }

    renderPagination(totalCount) {
        const pagination = document.getElementById('pagination');
        if (!pagination) return;

        const totalPages = Math.ceil(totalCount / this.pageSize);
        if (totalPages <= 1) {
            pagination.innerHTML = '';
            return;
        }

        let paginationHTML = '';
        
        // Previous button
        paginationHTML += `
            <li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="ordersManager.goToPage(${this.currentPage - 1})">
                    <i class="bi bi-chevron-left"></i>
                </a>
            </li>
        `;

        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || (i >= this.currentPage - 2 && i <= this.currentPage + 2)) {
                paginationHTML += `
                    <li class="page-item ${i === this.currentPage ? 'active' : ''}">
                        <a class="page-link" href="#" onclick="ordersManager.goToPage(${i})">${i}</a>
                    </li>
                `;
            } else if (i === this.currentPage - 3 || i === this.currentPage + 3) {
                paginationHTML += '<li class="page-item disabled"><span class="page-link">...</span></li>';
            }
        }

        // Next button
        paginationHTML += `
            <li class="page-item ${this.currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="ordersManager.goToPage(${this.currentPage + 1})">
                    <i class="bi bi-chevron-right"></i>
                </a>
            </li>
        `;

        pagination.innerHTML = paginationHTML;
    }

    goToPage(page) {
        if (page < 1) return;
        this.currentPage = page;
        this.loadOrders();
    }

    toggleSelectAll(checked) {
        const checkboxes = document.querySelectorAll('.order-checkbox');
        checkboxes.forEach(checkbox => {
            checkbox.checked = checked;
            this.toggleOrderSelection(checkbox.value, checked);
        });
    }

    toggleOrderSelection(orderId, selected) {
        if (selected) {
            this.selectedOrders.add(orderId);
        } else {
            this.selectedOrders.delete(orderId);
        }
        this.updateBulkActions();
    }

    updateBulkActions() {
        const bulkActions = document.getElementById('bulkActions');
        const selectedCount = document.getElementById('selectedCount');
        
        if (this.selectedOrders.size > 0) {
            bulkActions.style.display = 'flex';
            selectedCount.textContent = `${this.selectedOrders.size} sipariş seçildi`;
        } else {
            bulkActions.style.display = 'none';
        }
    }

    showBulkStatusUpdate() {
        if (this.selectedOrders.size === 0) return;
        
        // Reset modal
        document.getElementById('newStatus').value = 'Pending';
        document.getElementById('statusNote').value = '';
        document.getElementById('sendNotification').checked = true;
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('statusUpdateModal'));
        modal.show();
    }

    async updateOrderStatus() {
        const newStatus = document.getElementById('newStatus').value;
        const note = document.getElementById('statusNote').value;
        const sendNotification = document.getElementById('sendNotification').checked;

        try {
            const orderIds = Array.from(this.selectedOrders);
            const promises = orderIds.map(orderId => 
                this.updateSingleOrderStatus(orderId, newStatus, note, sendNotification)
            );

            await Promise.all(promises);
            
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('statusUpdateModal'));
            modal.hide();
            
            // Refresh orders
            this.loadOrders();
            
            // Show success message
            this.showSuccess(`${orderIds.length} sipariş durumu başarıyla güncellendi.`);
            
        } catch (error) {
            console.error('Error updating order status:', error);
            this.showError('Sipariş durumu güncellenirken bir hata oluştu.');
        }
    }

    async updateSingleOrderStatus(orderId, status = null, note = '', sendNotification = true) {
        const newStatus = status || document.getElementById('newStatus').value;
        const statusNote = note || document.getElementById('statusNote').value;
        const notify = sendNotification || document.getElementById('sendNotification').checked;

        try {
            const response = await fetch(`/api/orders/${orderId}/status`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    status: newStatus,
                    note: statusNote,
                    sendNotification: notify
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return await response.json();
            
        } catch (error) {
            console.error('Error updating order status:', error);
            throw error;
        }
    }

    applyAdvancedFilters() {
        this.filters.minAmount = document.getElementById('minAmount').value;
        this.filters.maxAmount = document.getElementById('maxAmount').value;
        this.filters.paymentMethod = document.getElementById('paymentMethodFilter').value;
        this.filters.shippingCompany = document.getElementById('shippingCompanyFilter').value;
        
        this.currentPage = 1;
        this.loadOrders();
        
        // Close modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('filterModal'));
        modal.hide();
    }

    async exportSelectedOrders() {
        if (this.selectedOrders.size === 0) return;
        
        try {
            const orderIds = Array.from(this.selectedOrders);
            const response = await fetch('/api/orders/export', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({ orderIds })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `siparisler_${new Date().toISOString().split('T')[0]}.xlsx`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            
        } catch (error) {
            console.error('Error exporting orders:', error);
            this.showError('Siparişler dışa aktarılırken bir hata oluştu.');
        }
    }

    // Utility methods
    formatDate(dateString) {
        if (!dateString) return '-';
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR');
    }

    getStatusBadgeClass(status) {
        const classes = {
            'Pending': 'bg-warning',
            'Processing': 'bg-info',
            'Shipped': 'bg-primary',
            'Delivered': 'bg-success',
            'Cancelled': 'bg-danger'
        };
        return classes[status] || 'bg-secondary';
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

    getPaymentBadgeClass(status) {
        const classes = {
            'Pending': 'bg-warning',
            'Paid': 'bg-success',
            'Failed': 'bg-danger',
            'Refunded': 'bg-info'
        };
        return classes[status] || 'bg-secondary';
    }

    getPaymentText(status) {
        const texts = {
            'Pending': 'Beklemede',
            'Paid': 'Ödendi',
            'Failed': 'Başarısız',
            'Refunded': 'İade Edildi'
        };
        return texts[status] || status;
    }

    debounce(func, wait) {
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

    getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    showLoading() {
        // Add loading indicator
        const tbody = document.getElementById('ordersTableBody');
        if (tbody) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-4">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Yükleniyor...</span>
                        </div>
                    </td>
                </tr>
            `;
        }
    }

    hideLoading() {
        // Loading is handled by renderOrders
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
    window.ordersManager = new SellerOrdersManager();
});
