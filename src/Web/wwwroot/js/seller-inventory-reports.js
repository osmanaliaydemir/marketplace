/**
 * Seller Inventory Reports JavaScript
 * Handles inventory analytics, stock alerts, and reporting
 */

class SellerInventoryReportsManager {
    constructor() {
        this.charts = {};
        this.currentFilters = {
            category: '',
            stockStatus: '',
            productStatus: '',
            searchTerm: ''
        };
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadInventoryData();
        this.loadCategories();
    }

    bindEvents() {
        // Filter events
        document.getElementById('categoryFilter')?.addEventListener('change', (e) => {
            this.currentFilters.category = e.target.value;
            this.loadInventoryData();
        });

        document.getElementById('stockStatusFilter')?.addEventListener('change', (e) => {
            this.currentFilters.stockStatus = e.target.value;
            this.loadInventoryData();
        });

        document.getElementById('productStatusFilter')?.addEventListener('change', (e) => {
            this.currentFilters.productStatus = e.target.value;
            this.loadInventoryData();
        });

        document.getElementById('searchInput')?.addEventListener('input', this.debounce((e) => {
            this.currentFilters.searchTerm = e.target.value;
            this.loadInventoryData();
        }, 500));

        // Button events
        document.getElementById('refreshBtn')?.addEventListener('click', () => {
            this.loadInventoryData();
        });

        document.getElementById('exportBtn')?.addEventListener('click', () => {
            this.exportInventoryReport();
        });
    }

    async loadCategories() {
        try {
            const response = await fetch('/api/categories', {
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const categories = await response.json();
            this.populateCategoryFilter(categories);
            
        } catch (error) {
            console.error('Error loading categories:', error);
        }
    }

    populateCategoryFilter(categories) {
        const select = document.getElementById('categoryFilter');
        if (!select) return;

        // Keep the first option (Tüm Kategoriler)
        const firstOption = select.options[0];
        select.innerHTML = '';
        select.appendChild(firstOption);

        categories.forEach(category => {
            const option = document.createElement('option');
            option.value = category.id;
            option.textContent = category.name;
            select.appendChild(option);
        });
    }

    async loadInventoryData() {
        try {
            this.showLoading();
            
            const queryParams = new URLSearchParams({
                categoryId: this.currentFilters.category,
                stockStatus: this.currentFilters.stockStatus,
                productStatus: this.currentFilters.productStatus,
                searchTerm: this.currentFilters.searchTerm
            });

            const response = await fetch(`/api/reports/inventory?${queryParams}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            this.updateKPIs(data);
            this.renderCharts(data);
            this.renderStockAlerts(data.stockAlerts || []);
            this.renderStockMovements(data.stockMovements || []);
            
        } catch (error) {
            console.error('Error loading inventory data:', error);
            this.showError('Stok verileri yüklenirken bir hata oluştu.');
        } finally {
            this.hideLoading();
        }
    }

    updateKPIs(data) {
        // Update KPI cards
        document.getElementById('totalProducts')?.textContent = data.totalProducts || 0;
        document.getElementById('inStockProducts')?.textContent = data.inStockProducts || 0;
        document.getElementById('lowStockProducts')?.textContent = data.lowStockProducts || 0;
        document.getElementById('outOfStockProducts')?.textContent = data.outOfStockProducts || 0;
    }

    renderCharts(data) {
        this.renderStockStatusChart(data.stockStatusDistribution || []);
        this.renderCategoryStockChart(data.categoryStockDistribution || []);
        this.renderStockCostChart(data.stockCostAnalysis || []);
        this.renderStockValueTrendChart(data.stockValueTrend || []);
    }

    renderStockStatusChart(stockData) {
        const ctx = document.getElementById('stockStatusChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.stockStatus) {
            this.charts.stockStatus.destroy();
        }

        const labels = ['Stokta', 'Düşük Stok', 'Stok Yok'];
        const data = [
            stockData.inStock || 0,
            stockData.lowStock || 0,
            stockData.outOfStock || 0
        ];

        this.charts.stockStatus = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: [
                        '#28a745', // Green for in stock
                        '#ffc107', // Yellow for low stock
                        '#dc3545'  // Red for out of stock
                    ]
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'bottom'
                    },
                    title: {
                        display: true,
                        text: 'Stok Durumu Dağılımı'
                    }
                }
            }
        });
    }

    renderCategoryStockChart(categoryData) {
        const ctx = document.getElementById('categoryStockChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.categoryStock) {
            this.charts.categoryStock.destroy();
        }

        const labels = categoryData.map(item => item.categoryName);
        const data = categoryData.map(item => item.productCount);

        this.charts.categoryStock = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Ürün Sayısı',
                    data: data,
                    backgroundColor: 'rgba(54, 162, 235, 0.8)'
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: 'Kategori Bazında Stok'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    renderStockCostChart(costData) {
        const ctx = document.getElementById('stockCostChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.stockCost) {
            this.charts.stockCost.destroy();
        }

        const labels = costData.map(item => item.categoryName);
        const data = costData.map(item => item.totalCost);

        this.charts.stockCost = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Stok Maliyeti (₺)',
                    data: data,
                    backgroundColor: 'rgba(255, 99, 132, 0.8)'
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: 'Stok Maliyeti Analizi'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    renderStockValueTrendChart(trendData) {
        const ctx = document.getElementById('stockValueTrendChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.stockValueTrend) {
            this.charts.stockValueTrend.destroy();
        }

        const labels = trendData.map(item => this.formatDate(item.date));
        const data = trendData.map(item => item.totalValue);

        this.charts.stockValueTrend = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Stok Değeri (₺)',
                    data: data,
                    borderColor: 'rgb(75, 192, 192)',
                    backgroundColor: 'rgba(75, 192, 192, 0.1)',
                    fill: true
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: false
                    },
                    title: {
                        display: true,
                        text: 'Stok Değeri Trendi'
                    }
                },
                scales: {
                    x: {
                        display: true,
                        title: {
                            display: true,
                            text: 'Tarih'
                        }
                    },
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Stok Değeri (₺)'
                        }
                    }
                }
            }
        });
    }

    renderStockAlerts(alerts) {
        const tbody = document.getElementById('stockAlertsTable');
        if (!tbody) return;

        if (alerts.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center py-3">
                        <em class="text-muted">Stok uyarısı bulunmuyor.</em>
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = alerts.map(alert => `
            <tr>
                <td>
                    <div class="d-flex align-items-center">
                        ${alert.imageUrl ? `<img src="${alert.imageUrl}" alt="${alert.productName}" class="me-3" style="width: 40px; height: 40px; object-fit: cover;">` : ''}
                        <div>
                            <strong>${alert.productName}</strong>
                            ${alert.sku ? `<br><small class="text-muted">SKU: ${alert.sku}</small>` : ''}
                        </div>
                    </div>
                </td>
                <td>${alert.categoryName || 'Kategorisiz'}</td>
                <td>
                    <span class="badge ${this.getStockBadgeClass(alert.currentStock)}">
                        ${alert.currentStock}
                    </span>
                </td>
                <td>${alert.minimumStock || 0}</td>
                <td>
                    <span class="badge ${this.getStockAlertBadgeClass(alert.stockLevel)}">
                        ${this.getStockAlertText(alert.stockLevel)}
                    </span>
                </td>
                <td>${this.formatDate(alert.lastUpdated)}</td>
                <td>
                    <div class="btn-group btn-group-sm">
                        <a href="/satici/urunler/${alert.productId}/duzenle" class="btn btn-outline-primary btn-sm">
                            <i class="bi bi-pencil"></i>
                        </a>
                        <button type="button" class="btn btn-outline-warning btn-sm" 
                                onclick="inventoryManager.addStock(${alert.productId})">
                            <i class="bi bi-plus-circle"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `).join('');
    }

    renderStockMovements(movements) {
        const tbody = document.getElementById('stockMovementsTable');
        if (!tbody) return;

        if (movements.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center py-3">
                        <em class="text-muted">Stok hareketi bulunmuyor.</em>
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = movements.map(movement => `
            <tr>
                <td>${this.formatDateTime(movement.timestamp)}</td>
                <td>
                    <div class="d-flex align-items-center">
                        ${movement.imageUrl ? `<img src="${movement.imageUrl}" alt="${movement.productName}" class="me-3" style="width: 30px; height: 30px; object-fit: cover;">` : ''}
                        <div>
                            <strong>${movement.productName}</strong>
                        </div>
                    </div>
                </td>
                <td>
                    <span class="badge ${this.getMovementBadgeClass(movement.type)}">
                        ${this.getMovementText(movement.type)}
                    </span>
                </td>
                <td>${movement.quantity}</td>
                <td>${movement.previousStock}</td>
                <td>${movement.newStock}</td>
                <td>${movement.note || '-'}</td>
            </tr>
        `).join('');
    }

    async addStock(productId) {
        const quantity = prompt('Eklemek istediğiniz stok miktarını girin:');
        if (!quantity || isNaN(quantity) || quantity <= 0) return;

        try {
            const response = await fetch(`/api/products/${productId}/stock`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({
                    quantity: parseInt(quantity),
                    note: 'Manuel stok ekleme'
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            // Refresh inventory data
            await this.loadInventoryData();
            this.showSuccess('Stok başarıyla eklendi.');
            
        } catch (error) {
            console.error('Error adding stock:', error);
            this.showError('Stok eklenirken bir hata oluştu.');
        }
    }

    async exportInventoryReport() {
        try {
            const queryParams = new URLSearchParams({
                categoryId: this.currentFilters.category,
                stockStatus: this.currentFilters.stockStatus,
                productStatus: this.currentFilters.productStatus,
                searchTerm: this.currentFilters.searchTerm
            });

            const response = await fetch(`/api/reports/inventory/export?${queryParams}`, {
                method: 'GET',
                headers: {
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `stok_raporu_${new Date().toISOString().split('T')[0]}.xlsx`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            
        } catch (error) {
            console.error('Error exporting inventory report:', error);
            this.showError('Stok raporu dışa aktarılırken bir hata oluştu.');
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

    getStockBadgeClass(stock) {
        if (stock > 10) return 'bg-success';
        if (stock > 0) return 'bg-warning';
        return 'bg-danger';
    }

    getStockAlertBadgeClass(level) {
        switch (level) {
            case 'critical': return 'bg-danger';
            case 'low': return 'bg-warning';
            case 'normal': return 'bg-success';
            default: return 'bg-secondary';
        }
    }

    getStockAlertText(level) {
        switch (level) {
            case 'critical': return 'Kritik';
            case 'low': return 'Düşük';
            case 'normal': return 'Normal';
            default: return level;
        }
    }

    getMovementBadgeClass(type) {
        switch (type) {
            case 'in': return 'bg-success';
            case 'out': return 'bg-danger';
            case 'adjustment': return 'bg-info';
            default: return 'bg-secondary';
        }
    }

    getMovementText(type) {
        switch (type) {
            case 'in': return 'Giriş';
            case 'out': return 'Çıkış';
            case 'adjustment': return 'Düzeltme';
            default: return type;
        }
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
        // Add loading indicators to charts
        const chartContainers = document.querySelectorAll('canvas');
        chartContainers.forEach(canvas => {
            const loadingDiv = document.createElement('div');
            loadingDiv.className = 'text-center py-4';
            loadingDiv.innerHTML = `
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
            `;
            canvas.parentNode.appendChild(loadingDiv);
        });
    }

    hideLoading() {
        // Remove loading indicators
        const loadingIndicators = document.querySelectorAll('.spinner-border');
        loadingIndicators.forEach(indicator => {
            indicator.parentNode.remove();
        });
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
    window.inventoryManager = new SellerInventoryReportsManager();
});
