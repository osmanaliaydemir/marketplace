/**
 * Seller Dashboard Manager
 * Handles dashboard data loading, charts, and real-time updates
 */
class SellerDashboardManager {
    constructor() {
        this.charts = {};
        this.currentPeriod = 'week';
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadDashboardData();
        this.setupCharts();
    }

    bindEvents() {
        // Period selector buttons
        document.querySelectorAll('[data-period]').forEach(btn => {
            btn.addEventListener('click', (e) => {
                this.setActivePeriod(e.target.dataset.period);
            });
        });

        // Refresh button
        document.getElementById('refreshBtn')?.addEventListener('click', () => {
            this.refreshData();
        });
    }

    setActivePeriod(period) {
        this.currentPeriod = period;
        
        // Update button states
        document.querySelectorAll('[data-period]').forEach(btn => {
            btn.classList.toggle('active', btn.dataset.period === period);
        });

        // Reload sales chart with new period
        this.loadSalesData();
    }

    async loadDashboardData() {
        try {
            this.showLoading(true);
            
            // Load all dashboard data in parallel
            const [ordersData, productsData, salesData] = await Promise.all([
                this.fetchOrdersData(),
                this.fetchProductsData(),
                this.fetchSalesData()
            ]);

            this.updateKPICards(ordersData, productsData, salesData);
            this.updateRecentOrders(ordersData);
            this.updateStockAlerts(productsData);
            this.updatePerformanceMetrics(ordersData, salesData);
            
            this.showDashboard();
            
        } catch (error) {
            console.error('Error loading dashboard data:', error);
            this.showError('Dashboard verileri yüklenirken bir hata oluştu');
        } finally {
            this.showLoading(false);
        }
    }

    async fetchOrdersData() {
        try {
            const response = await fetch('/api/orders/seller?page=1&pageSize=10', {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            
            if (response.ok) {
                return await response.json();
            }
            return { items: [], totalCount: 0 };
        } catch (error) {
            console.error('Error fetching orders:', error);
            return { items: [], totalCount: 0 };
        }
    }

    async fetchProductsData() {
        try {
            const response = await fetch('/api/products/mine?page=1&pageSize=100', {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            
            if (response.ok) {
                return await response.json();
            }
            return { items: [], totalCount: 0 };
        } catch (error) {
            console.error('Error fetching products:', error);
            return { items: [], totalCount: 0 };
        }
    }

    async fetchSalesData() {
        try {
            const response = await fetch(`/api/reports/sales?period=${this.currentPeriod}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });
            
            if (response.ok) {
                return await response.json();
            }
            return { dailySales: 0, dailyOrders: 0, salesTrend: [], categoryDistribution: [] };
        } catch (error) {
            console.error('Error fetching sales data:', error);
            return { dailySales: 0, dailyOrders: 0, salesTrend: [], categoryDistribution: [] };
        }
    }

    updateKPICards(ordersData, productsData, salesData) {
        // Daily Sales
        const dailySales = salesData.dailySales || 0;
        document.getElementById('dailySales').textContent = `₺${dailySales.toLocaleString()}`;
        
        const salesChange = salesData.salesChange || 0;
        const salesIcon = document.getElementById('dailySalesIcon');
        const salesChangeSpan = document.getElementById('dailySalesChange');
        
        salesChangeSpan.textContent = `${Math.abs(salesChange)}%`;
        salesIcon.className = salesChange >= 0 ? 'fas fa-arrow-up' : 'fas fa-arrow-down';
        salesChangeSpan.className = salesChange >= 0 ? 'text-success' : 'text-danger';

        // Daily Orders
        const dailyOrders = salesData.dailyOrders || 0;
        document.getElementById('dailyOrders').textContent = dailyOrders;
        
        const ordersChange = salesData.ordersChange || 0;
        const ordersIcon = document.getElementById('dailyOrdersIcon');
        const ordersChangeSpan = document.getElementById('dailyOrdersChange');
        
        ordersChangeSpan.textContent = `${Math.abs(ordersChange)}%`;
        ordersIcon.className = ordersChange >= 0 ? 'fas fa-arrow-up' : 'fas fa-arrow-down';
        ordersChangeSpan.className = ordersChange >= 0 ? 'text-success' : 'text-danger';

        // Total Products
        const totalProducts = productsData.totalCount || 0;
        const activeProducts = productsData.items?.filter(p => p.isActive).length || 0;
        
        document.getElementById('totalProducts').textContent = totalProducts;
        document.getElementById('activeProducts').textContent = `${activeProducts} aktif`;

        // Pending Orders
        const pendingOrders = ordersData.items?.filter(o => o.status === 'Pending').length || 0;
        document.getElementById('pendingOrders').textContent = pendingOrders;
    }

    updateRecentOrders(ordersData) {
        const tbody = document.getElementById('recentOrdersTable');
        if (!tbody) return;

        const orders = ordersData.items || [];
        
        if (orders.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2"></i>
                        <br>Henüz sipariş bulunmuyor
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = orders.map(order => `
            <tr>
                <td>
                    <strong>#${order.orderNumber || order.id}</strong>
                </td>
                <td>
                    <div class="d-flex align-items-center">
                        <div class="avatar-sm me-2">
                            <i class="fas fa-user-circle fa-lg text-muted"></i>
                        </div>
                        <div>
                            <div class="fw-medium">${order.customerName || 'Müşteri'}</div>
                            <small class="text-muted">${order.customerEmail || ''}</small>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="d-flex align-items-center">
                        <img src="${order.items?.[0]?.productImage || '/images/placeholder.png'}" 
                             alt="Ürün" class="rounded me-2" style="width: 32px; height: 32px; object-fit: cover;">
                        <div>
                            <div class="fw-medium">${order.items?.[0]?.productName || 'Ürün'}</div>
                            <small class="text-muted">${order.items?.length || 0} ürün</small>
                        </div>
                    </div>
                </td>
                <td>
                    <strong class="text-primary">₺${(order.totalAmount || 0).toLocaleString()}</strong>
                </td>
                <td>
                    <span class="badge ${this.getStatusBadgeClass(order.status)}">
                        ${this.getStatusText(order.status)}
                    </span>
                </td>
                <td>
                    <small class="text-muted">
                        ${this.formatDate(order.createdAt)}
                    </small>
                </td>
                <td>
                    <a href="/satici/siparisler/${order.id}" class="btn btn-sm btn-outline-primary">
                        <i class="fas fa-eye"></i>
                    </a>
                </td>
            </tr>
        `).join('');
    }

    updateStockAlerts(productsData) {
        const stockAlertsDiv = document.getElementById('stockAlerts');
        if (!stockAlertsDiv) return;

        const products = productsData.items || [];
        const lowStockProducts = products.filter(p => p.stockQty <= 10 && p.stockQty > 0);
        const outOfStockProducts = products.filter(p => p.stockQty === 0);

        if (lowStockProducts.length === 0 && outOfStockProducts.length === 0) {
            stockAlertsDiv.innerHTML = `
                <div class="text-center text-muted py-3">
                    <i class="fas fa-check-circle fa-2x text-success mb-2"></i>
                    <br>Stok uyarısı bulunmuyor
                </div>
            `;
            return;
        }

        let alertsHtml = '';

        if (outOfStockProducts.length > 0) {
            alertsHtml += `
                <div class="alert alert-danger alert-sm mb-2">
                    <i class="fas fa-exclamation-triangle me-2"></i>
                    <strong>${outOfStockProducts.length}</strong> ürün stokta yok
                </div>
            `;
        }

        if (lowStockProducts.length > 0) {
            alertsHtml += `
                <div class="alert alert-warning alert-sm mb-2">
                    <i class="fas fa-exclamation-circle me-2"></i>
                    <strong>${lowStockProducts.length}</strong> ürün stoku az
                </div>
            `;
        }

        // Show first few products
        const showProducts = [...outOfStockProducts, ...lowStockProducts].slice(0, 3);
        showProducts.forEach(product => {
            alertsHtml += `
                <div class="d-flex align-items-center justify-content-between py-2 border-bottom">
                    <div class="d-flex align-items-center">
                        <img src="${product.primaryImageUrl || '/images/placeholder.png'}" 
                             alt="${product.name}" class="rounded me-2" 
                             style="width: 24px; height: 24px; object-fit: cover;">
                        <div>
                            <div class="small fw-medium">${product.name}</div>
                            <small class="text-muted">Stok: ${product.stockQty}</small>
                        </div>
                    </div>
                    <a href="/satici/urunler/${product.id}/duzenle" class="btn btn-sm btn-outline-primary">
                        <i class="fas fa-edit"></i>
                    </a>
                </div>
            `;
        });

        stockAlertsDiv.innerHTML = alertsHtml;
    }

    updatePerformanceMetrics(ordersData, salesData) {
        // Average Order Value
        const orders = ordersData.items || [];
        const avgOrderValue = orders.length > 0 
            ? orders.reduce((sum, order) => sum + (order.totalAmount || 0), 0) / orders.length 
            : 0;
        document.getElementById('avgOrderValue').textContent = `₺${avgOrderValue.toLocaleString()}`;

        // Conversion Rate (placeholder)
        const conversionRate = salesData.conversionRate || 0;
        document.getElementById('conversionRate').textContent = `${conversionRate}%`;

        // Customer Satisfaction (placeholder)
        const customerSatisfaction = salesData.customerSatisfaction || 0;
        document.getElementById('customerSatisfaction').textContent = `${customerSatisfaction}/5`;

        // Response Time (placeholder)
        const responseTime = salesData.responseTime || 0;
        document.getElementById('responseTime').textContent = `${responseTime}s`;
    }

    setupCharts() {
        this.setupSalesChart();
        this.setupCategoryChart();
    }

    setupSalesChart() {
        const ctx = document.getElementById('salesChart');
        if (!ctx) return;

        this.charts.sales = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'Satış Tutarı (₺)',
                    data: [],
                    borderColor: 'rgb(75, 192, 192)',
                    backgroundColor: 'rgba(75, 192, 192, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '₺' + value.toLocaleString();
                            }
                        }
                    }
                }
            }
        });
    }

    setupCategoryChart() {
        const ctx = document.getElementById('categoryChart');
        if (!ctx) return;

        this.charts.category = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: [],
                datasets: [{
                    data: [],
                    backgroundColor: [
                        '#FF6384',
                        '#36A2EB',
                        '#FFCE56',
                        '#4BC0C0',
                        '#9966FF',
                        '#FF9F40'
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    }

    async loadSalesData() {
        try {
            const salesData = await this.fetchSalesData();
            
            // Update sales chart
            if (this.charts.sales && salesData.salesTrend) {
                this.charts.sales.data.labels = salesData.salesTrend.map(item => item.date);
                this.charts.sales.data.datasets[0].data = salesData.salesTrend.map(item => item.amount);
                this.charts.sales.update();
            }

            // Update category chart
            if (this.charts.category && salesData.categoryDistribution) {
                this.charts.category.data.labels = salesData.categoryDistribution.map(item => item.category);
                this.charts.category.data.datasets[0].data = salesData.categoryDistribution.map(item => item.count);
                this.charts.category.update();
            }
        } catch (error) {
            console.error('Error loading sales data:', error);
        }
    }

    refreshData() {
        this.loadDashboardData();
    }

    // Utility methods
    getStatusBadgeClass(status) {
        const statusMap = {
            'Pending': 'bg-warning',
            'Confirmed': 'bg-info',
            'Processing': 'bg-primary',
            'Shipped': 'bg-info',
            'Delivered': 'bg-success',
            'Cancelled': 'bg-danger',
            'Returned': 'bg-secondary'
        };
        return statusMap[status] || 'bg-secondary';
    }

    getStatusText(status) {
        const statusMap = {
            'Pending': 'Bekliyor',
            'Confirmed': 'Onaylandı',
            'Processing': 'Hazırlanıyor',
            'Shipped': 'Kargoda',
            'Delivered': 'Teslim Edildi',
            'Cancelled': 'İptal Edildi',
            'Returned': 'İade Edildi'
        };
        return statusMap[status] || status;
    }

    formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    showLoading(show) {
        const spinner = document.getElementById('loadingSpinner');
        const content = document.getElementById('dashboardContent');
        
        if (show) {
            spinner.style.display = 'block';
            content.style.display = 'none';
        } else {
            spinner.style.display = 'none';
            content.style.display = 'block';
        }
    }

    showDashboard() {
        document.getElementById('dashboardContent').style.display = 'block';
    }

    showError(message) {
        // Create temporary error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger alert-dismissible fade show position-fixed';
        errorDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        errorDiv.innerHTML = `
            <i class="fas fa-exclamation-triangle me-2"></i>${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(errorDiv);
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            errorDiv.remove();
        }, 5000);
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardManager = new SellerDashboardManager();
});
