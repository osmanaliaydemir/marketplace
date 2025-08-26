// Store Application Form JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initializeStoreApplication();
});

function initializeStoreApplication() {
    let currentStep = 1;
    const totalSteps = 3;
    
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const submitBtn = document.getElementById('submitBtn');
    const form = document.getElementById('storeApplicationForm');
    
    // Event listeners
    if (nextBtn) nextBtn.addEventListener('click', () => nextStep());
    if (prevBtn) prevBtn.addEventListener('click', () => prevStep());
    if (form) form.addEventListener('submit', handleFormSubmit);
    
    // Category change handler
    const primaryCategory = document.getElementById('primaryCategory');
    if (primaryCategory) {
        primaryCategory.addEventListener('change', updateSecondaryCategories);
    }
    
    // Form validation
    setupFormValidation();
}

// Step Navigation
function nextStep() {
    const currentStepElement = document.querySelector('.form-step.active');
    const currentStepNumber = parseInt(currentStepElement.id.split('-')[2]);
    
    if (validateCurrentStep(currentStepNumber)) {
        if (currentStepNumber < 3) {
            showStep(currentStepNumber + 1);
        }
    }
}

function prevStep() {
    const currentStepElement = document.querySelector('.form-step.active');
    const currentStepNumber = parseInt(currentStepElement.id.split('-')[2]);
    
    if (currentStepNumber > 1) {
        showStep(currentStepNumber - 1);
    }
}

function showStep(stepNumber) {
    // Hide all steps
    document.querySelectorAll('.form-step').forEach(step => {
        step.classList.remove('active');
    });
    
    // Show current step
    const currentStep = document.getElementById(`form-step-${stepNumber}`);
    if (currentStep) {
        currentStep.classList.add('active');
    }
    
    // Update progress indicators
    updateProgressSteps(stepNumber);
    
    // Update navigation buttons
    updateNavigationButtons(stepNumber);
    
    // Scroll to top of form
    document.querySelector('.application-form').scrollIntoView({ 
        behavior: 'smooth', 
        block: 'start' 
    });
}

function updateProgressSteps(activeStep) {
    // Remove active class from all steps
    document.querySelectorAll('.step-item').forEach(item => {
        item.classList.remove('active', 'completed');
    });
    
    // Add appropriate classes
    for (let i = 1; i <= 3; i++) {
        const stepItem = document.getElementById(`step-${i}`);
        if (i < activeStep) {
            stepItem.classList.add('completed');
        } else if (i === activeStep) {
            stepItem.classList.add('active');
        }
    }
}

function updateNavigationButtons(stepNumber) {
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const submitBtn = document.getElementById('submitBtn');
    
    if (stepNumber === 1) {
        prevBtn.style.display = 'none';
        nextBtn.style.display = 'block';
        submitBtn.style.display = 'none';
    } else if (stepNumber === 2) {
        prevBtn.style.display = 'block';
        nextBtn.style.display = 'block';
        submitBtn.style.display = 'none';
    } else if (stepNumber === 3) {
        prevBtn.style.display = 'block';
        nextBtn.style.display = 'none';
        submitBtn.style.display = 'block';
    }
}

// Form Validation
function validateCurrentStep(stepNumber) {
    const currentStepElement = document.getElementById(`form-step-${stepNumber}`);
    const requiredFields = currentStepElement.querySelectorAll('[required]');
    let isValid = true;
    
    // Clear previous error messages
    currentStepElement.querySelectorAll('.is-invalid').forEach(field => {
        field.classList.remove('is-invalid');
    });
    
    // Validate required fields
    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('is-invalid');
            isValid = false;
        }
        
        // Special validation for specific field types
        if (field.type === 'email' && field.value) {
            if (!isValidEmail(field.value)) {
                field.classList.add('is-invalid');
                isValid = false;
            }
        }
        
        if (field.type === 'url' && field.value) {
            if (!isValidUrl(field.value)) {
                field.classList.add('is-invalid');
                isValid = false;
            }
        }
    });
    
    // Special validation for step 3
    if (stepNumber === 3) {
        const termsAccepted = document.getElementById('termsAccepted');
        if (!termsAccepted.checked) {
            termsAccepted.classList.add('is-invalid');
            isValid = false;
        }
    }
    
    if (!isValid) {
        showStepValidationMessage(stepNumber);
    }
    
    return isValid;
}

function setupFormValidation() {
    // Real-time validation
    document.querySelectorAll('input, select, textarea').forEach(field => {
        field.addEventListener('blur', function() {
            validateField(this);
        });
        
        field.addEventListener('input', function() {
            if (this.classList.contains('is-invalid')) {
                validateField(this);
            }
        });
    });
}

function validateField(field) {
    const value = field.value.trim();
    
    // Remove previous validation state
    field.classList.remove('is-valid', 'is-invalid');
    
    // Check if required
    if (field.hasAttribute('required') && !value) {
        field.classList.add('is-invalid');
        return false;
    }
    
    // Type-specific validation
    if (field.type === 'email' && value) {
        if (!isValidEmail(value)) {
            field.classList.add('is-invalid');
            return false;
        }
    }
    
    if (field.type === 'url' && value) {
        if (!isValidUrl(value)) {
            field.classList.add('is-invalid');
            return false;
        }
    }
    
    // Length validation
    if (field.hasAttribute('maxlength')) {
        const maxLength = parseInt(field.getAttribute('maxlength'));
        if (value.length > maxLength) {
            field.classList.add('is-invalid');
            return false;
        }
    }
    
    // If all validations pass
    if (value) {
        field.classList.add('is-valid');
    }
    
    return true;
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function isValidUrl(url) {
    try {
        new URL(url);
        return true;
    } catch {
        return false;
    }
}

function showStepValidationMessage(stepNumber) {
    const stepNames = {
        1: 'İşletme Bilgileri',
        2: 'Kategori & Ürünler',
        3: 'İletişim & Onay'
    };
    
    const message = `${stepNames[stepNumber]} adımında eksik bilgiler var. Lütfen tüm gerekli alanları doldurun.`;
    
    // Show toast notification
    showToast(message, 'warning');
}

// Secondary Categories
function updateSecondaryCategories() {
    const primaryCategory = document.getElementById('primaryCategory');
    const secondaryCategory = document.getElementById('secondaryCategory');
    
    if (!primaryCategory || !secondaryCategory) return;
    
    const selectedCategory = primaryCategory.value;
    secondaryCategory.innerHTML = '<option value="">Seçiniz</option>';
    
    const secondaryCategories = getSecondaryCategories(selectedCategory);
    secondaryCategories.forEach(category => {
        const option = document.createElement('option');
        option.value = category.value;
        option.textContent = category.label;
        secondaryCategory.appendChild(option);
    });
}

function getSecondaryCategories(primaryCategory) {
    const categories = {
        'fashion': [
            { value: 'mens-clothing', label: 'Erkek Giyim' },
            { value: 'womens-clothing', label: 'Kadın Giyim' },
            { value: 'kids-clothing', label: 'Çocuk Giyim' },
            { value: 'shoes', label: 'Ayakkabı' },
            { value: 'accessories', label: 'Aksesuar' }
        ],
        'electronics': [
            { value: 'phones', label: 'Telefon' },
            { value: 'computers', label: 'Bilgisayar' },
            { value: 'tablets', label: 'Tablet' },
            { value: 'accessories', label: 'Aksesuar' },
            { value: 'gaming', label: 'Oyun' }
        ],
        'home': [
            { value: 'furniture', label: 'Mobilya' },
            { value: 'decor', label: 'Dekorasyon' },
            { value: 'kitchen', label: 'Mutfak' },
            { value: 'bathroom', label: 'Banyo' },
            { value: 'garden', label: 'Bahçe' }
        ],
        'beauty': [
            { value: 'skincare', label: 'Cilt Bakımı' },
            { value: 'makeup', label: 'Makyaj' },
            { value: 'haircare', label: 'Saç Bakımı' },
            { value: 'fragrances', label: 'Parfüm' },
            { value: 'tools', label: 'Aletler' }
        ]
    };
    
    return categories[primaryCategory] || [];
}

// Form Submission
function handleFormSubmit(event) {
    event.preventDefault();
    
    if (!validateAllSteps()) {
        showToast('Lütfen tüm gerekli alanları doldurun.', 'error');
        return;
    }
    
    // Show loading state
    const submitBtn = document.getElementById('submitBtn');
    const originalText = submitBtn.innerHTML;
    submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Gönderiliyor...';
    submitBtn.disabled = true;
    
    // Submit form - build real form data including antiforgery token
    const form = event.target;
    const formData = new FormData(form);
    
    submitForm(formData, submitBtn, originalText);
}

function validateAllSteps() {
    for (let i = 1; i <= 3; i++) {
        if (!validateCurrentStep(i)) {
            showStep(i);
            return false;
        }
    }
    return true;
}

async function submitForm(formData, submitBtn, originalText) {
    try {
        console.log('FormData içeriği:');
        for (let [key, value] of formData.entries()) {
            console.log(`${key}: ${value}`);
        }
        
        // AJAX (XMLHttpRequest) ile gönder
        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/magaza-basvurusu', true);
        xhr.withCredentials = true; // same-origin cookie'leri gönder
        
        // Anti-forgery token'ı header olarak ekle
        try {
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenInput && tokenInput.value) {
                xhr.setRequestHeader('RequestVerificationToken', tokenInput.value);
            }
        } catch {}
        
        xhr.onreadystatechange = async function() {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                console.log('Response status:', xhr.status);
                if (xhr.status >= 200 && xhr.status < 300) {
                    let result = {};
                    try {
                        result = JSON.parse(xhr.responseText || '{}');
                    } catch {}
                    console.log('Response result:', result);
                    if (result.success) {
                        showToast('Başvurunuz başarıyla gönderildi!', 'success');
                        setTimeout(() => {
                            window.location.href = '/magaza-basvurusu/basarili';
                        }, 2000);
                    } else {
                        showToast(result.message || 'Başvuru gönderilirken bir hata oluştu.', 'error');
                    }
                } else {
                    console.log('Error response:', xhr.responseText);
                    showToast('Başvuru gönderilirken bir hata oluştu. Lütfen tekrar deneyin.', 'error');
                }
            }
        };
        
        xhr.send(formData);
    } catch (error) {
        console.error('Form submission error:', error);
        showToast('Bağlantı hatası oluştu. Lütfen tekrar deneyin.', 'error');
    } finally {
        submitBtn.innerHTML = originalText;
        submitBtn.disabled = false;
    }
}

// Toast Notifications
function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast toast-${type} show`;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
        background: ${getToastColor(type)};
        color: white;
        padding: 1rem;
        border-radius: 0.375rem;
        box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
    `;
    
    toast.innerHTML = `
        <div class="d-flex align-items-center">
            <i class="fas ${getToastIcon(type)} me-2"></i>
            <span>${message}</span>
            <button type="button" class="btn-close btn-close-white ms-auto" onclick="this.parentElement.parentElement.remove()"></button>
        </div>
    `;
    
    document.body.appendChild(toast);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (toast.parentNode) {
            toast.remove();
        }
    }, 5000);
}

function getToastColor(type) {
    const colors = {
        success: '#198754',
        error: '#dc3545',
        warning: '#ffc107',
        info: '#0dcaf0'
    };
    return colors[type] || colors.info;
}

function getToastIcon(type) {
    const icons = {
        success: 'fa-check-circle',
        error: 'fa-exclamation-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };
    return icons[type] || icons.info;
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

// Auto-save form data
const autoSave = debounce(() => {
    const formData = new FormData(document.getElementById('storeApplicationForm'));
    const data = {};
    
    for (let [key, value] of formData.entries()) {
        data[key] = value;
    }
    
    localStorage.setItem('storeApplicationDraft', JSON.stringify(data));
}, 1000);

// Load saved form data
function loadSavedFormData() {
    const saved = localStorage.getItem('storeApplicationDraft');
    if (saved) {
        try {
            const data = JSON.parse(saved);
            Object.keys(data).forEach(key => {
                const field = document.querySelector(`[name="${key}"]`);
                if (field) {
                    if (field.type === 'checkbox') {
                        field.checked = data[key] === 'true';
                    } else {
                        field.value = data[key];
                    }
                }
            });
        } catch (error) {
            console.error('Error loading saved form data:', error);
        }
    }
}

// Load saved data when page loads
document.addEventListener('DOMContentLoaded', loadSavedFormData);

// Auto-save on form changes
document.addEventListener('input', autoSave);
document.addEventListener('change', autoSave);
