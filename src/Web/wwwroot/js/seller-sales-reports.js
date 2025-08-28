/**
 * Seller Sales Reports JavaScript
 * Handles sales analytics, charts, and reporting
 */

class SellerSalesReportsManager {
    constructor() {
        this.charts = {};
        this.currentFilters = {
            dateRange: '30',
            startDate: '',
            endDate: '',
            category: ''
        };
        this.init();
    }

    init() {
        this.bindEvents();
        this.setupDateDefaults();
        this.loadSalesData();
        this.loadCategories();
    }

    bindEvents() {
        // Filter events
        document.getElementById('dateRange')?.addEventListener('change', (e) => {
            this.currentFilters.dateRange = e.target.value;
            this.handleDateRangeChange(e.target.value);
            this.loadSalesData();
        });

        document.getElementById('startDate')?.addEventListener('change', (e) => {
            this.currentFilters.startDate = e.target.value;
            this.loadSalesData();
        });

        document.getElementById('endDate')?.addEventListener('change', (e) => {
            this.currentFilters.endDate = e.target.value;
            this.loadSalesData();
        });

        document.getElementById('categoryFilter')?.addEventListener('change', (e) => {
            this.currentFilters.category = e.target.value;
            this.loadSalesData();
        });

        // Button events
        document.getElementById('refreshBtn')?.addEventListener('click', () => {
            this.loadSalesData();
        });

        document.getElementById('exportBtn')?.addEventListener('click', () => {
            this.exportSalesReport();
        });
    }

    setupDateDefaults() {
        const today = new Date();
        const thirtyDaysAgo = new Date(today.getTime() - (30 * 24 * 60 * 60 * 1000));
        
        if (document.getElementById('startDate')) {
            document.getElementById('startDate').value = this.formatDate(thirtyDaysAgo);
            this.currentFilters.startDate = this.formatDate(thirtyDaysAgo);
        }
        
        if (document.getElementById('endDate')) {
            document.getElementById('endDate').value = this.formatDate(today);
            this.currentFilters.endDate = this.formatDate(today);
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
            
            // Set dates based on selection
            const today = new Date();
            let startDate = new Date();
            
            switch (value) {
                case '7':
                    startDate.setDate(today.getDate() - 7);
                    break;
                case '30':
                    startDate.setDate(today.getDate() - 30);
                    break;
                case '90':
                    startDate.setDate(today.getDate() - 90);
                    break;
                case '365':
                    startDate.setDate(today.getDate() - 365);
                    break;
            }
            
            this.currentFilters.startDate = this.formatDate(startDate);
            this.currentFilters.endDate = this.formatDate(today);
        }
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

    async loadSalesData() {
        try {
            this.showLoading();
            
            const queryParams = new URLSearchParams({
                startDate: this.currentFilters.startDate,
                endDate: this.currentFilters.endDate,
                categoryId: this.currentFilters.category
            });

            const response = await fetch(`/api/reports/sales?${queryParams}`, {
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
            this.renderTopProducts(data.topProducts || []);
            
        } catch (error) {
            console.error('Error loading sales data:', error);
            this.showError('Satış verileri yüklenirken bir hata oluştu.');
        } finally {
            this.hideLoading();
        }
    }

    updateKPIs(data) {
        // Update KPI cards
        document.getElementById('totalSales')?.textContent = `₺${(data.totalSales || 0).toFixed(2)}`;
        document.getElementById('totalOrders')?.textContent = data.totalOrders || 0;
        document.getElementById('avgOrderValue')?.textContent = `₺${(data.averageOrderValue || 0).toFixed(2)}`;
        document.getElementById('uniqueCustomers')?.textContent = data.uniqueCustomers || 0;
    }

    renderCharts(data) {
        this.renderSalesTrendChart(data.salesTrend || []);
        this.renderCategoryChart(data.categoryDistribution || []);
        this.renderCustomerSegmentChart(data.customerSegments || []);
        this.renderDailySalesChart(data.dailySales || []);
    }

    renderSalesTrendChart(salesTrend) {
        const ctx = document.getElementById('salesTrendChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.salesTrend) {
            this.charts.salesTrend.destroy();
        }

        const labels = salesTrend.map(item => this.formatDate(item.date));
        const salesData = salesTrend.map(item => item.amount);
        const ordersData = salesTrend.map(item => item.orderCount);

        this.charts.salesTrend = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Satış Tutarı (₺)',
                        data: salesData,
                        borderColor: 'rgb(75, 192, 192)',
                        backgroundColor: 'rgba(75, 192, 192, 0.1)',
                        yAxisID: 'y'
                    },
                    {
                        label: 'Sipariş Sayısı',
                        data: ordersData,
                        borderColor: 'rgb(255, 99, 132)',
                        backgroundColor: 'rgba(255, 99, 132, 0.1)',
                        yAxisID: 'y1'
                    }
                ]
            },
            options: {
                responsive: true,
                interaction: {
                    mode: 'index',
                    intersect: false,
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
                        type: 'linear',
                        display: true,
                        position: 'left',
                        title: {
                            display: true,
                            text: 'Satış Tutarı (₺)'
                        }
                    },
                    y1: {
                        type: 'linear',
                        display: true,
                        position: 'right',
                        title: {
                            display: true,
                            text: 'Sipariş Sayısı'
                        },
                        grid: {
                            drawOnChartArea: false,
                        },
                    }
                },
                plugins: {
                    title: {
                        display: true,
                        text: 'Satış Trendi'
                    }
                }
            }
        });
    }

    renderCategoryChart(categoryData) {
        const ctx = document.getElementById('categoryChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.category) {
            this.charts.category.destroy();
        }

        const labels = categoryData.map(item => item.categoryName);
        const data = categoryData.map(item => item.salesAmount);

        this.charts.category = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
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
                plugins: {
                    legend: {
                        position: 'bottom'
                    },
                    title: {
                        display: true,
                        text: 'Kategori Bazında Satış'
                    }
                }
            }
        });
    }

    renderCustomerSegmentChart(customerSegments) {
        const ctx = document.getElementById('customerSegmentChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.customerSegment) {
            this.charts.customerSegment.destroy();
        }

        const labels = customerSegments.map(item => item.segment);
        const data = customerSegments.map(item => item.count);

        this.charts.customerSegment = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Müşteri Sayısı',
                    data: data,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.8)',
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(255, 205, 86, 0.8)',
                        'rgba(75, 192, 192, 0.8)'
                    ]
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
                        text: 'Müşteri Segmentasyonu'
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

    renderDailySalesChart(dailySales) {
        const ctx = document.getElementById('dailySalesChart');
        if (!ctx) return;

        // Destroy existing chart
        if (this.charts.dailySales) {
            this.charts.dailySales.destroy();
        }

        const labels = dailySales.map(item => item.day);
        const data = dailySales.map(item => item.amount);

        this.charts.dailySales = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Günlük Satış (₺)',
                    data: data,
                    backgroundColor: 'rgba(75, 192, 192, 0.8)'
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
                        text: 'Günlük Satış Dağılımı'
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

    renderTopProducts(products) {
        const tbody = document.getElementById('topProductsTable');
        if (!tbody) return;

        if (products.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center py-3">
                        <em class="text-muted">Henüz satış verisi bulunmuyor.</em>
                    </td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = products.map(product => `
            <tr>
                <td>
                    <div class="d-flex align-items-center">
                        ${product.imageUrl ? `<img src="${product.imageUrl}" alt="${product.name}" class="me-3" style="width: 40px; height: 40px; object-fit: cover;">` : ''}
                        <div>
                            <strong>${product.name}</strong>
                            ${product.sku ? `<br><small class="text-muted">SKU: ${product.sku}</small>` : ''}
                        </div>
                    </div>
                </td>
                <td>${product.categoryName || 'Kategorisiz'}</td>
                <td><span class="badge bg-success">${product.soldQuantity || 0}</span></td>
                <td><strong>₺${(product.totalSales || 0).toFixed(2)}</strong></td>
                <td>₺${(product.averagePrice || 0).toFixed(2)}</td>
            </tr>
        `).join('');
    }

    async exportSalesReport() {
        try {
            const queryParams = new URLSearchParams({
                startDate: this.currentFilters.startDate,
                endDate: this.currentFilters.endDate,
                categoryId: this.currentFilters.category
            });

            const response = await fetch(`/api/reports/sales/export?${queryParams}`, {
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
            a.download = `satis_raporu_${new Date().toISOString().split('T')[0]}.xlsx`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            
        } catch (error) {
            console.error('Error exporting sales report:', error);
            this.showError('Satış raporu dışa aktarılırken bir hata oluştu.');
        }
    }

    // Utility methods
    formatDate(date) {
        if (!date) return '';
        const d = new Date(date);
        return d.toLocaleDateString('tr-TR');
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
    window.salesReportsManager = new SellerSalesReportsManager();
});
