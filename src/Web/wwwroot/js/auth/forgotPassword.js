// Forgot Password Form Validation
document.addEventListener('DOMContentLoaded', function() {
    const form = document.querySelector('form');
    const emailInput = document.getElementById('email');

    if (!form || !emailInput) return;

    form.addEventListener('submit', function(e) {
        if (!emailInput.value.trim()) {
            e.preventDefault();
            emailInput.classList.add('is-invalid');
            return;
        }

        if (!isValidEmail(emailInput.value)) {
            e.preventDefault();
            emailInput.classList.add('is-invalid');
            return;
        }

        emailInput.classList.remove('is-invalid');
    });

    emailInput.addEventListener('input', function() {
        if (this.value.trim() && isValidEmail(this.value)) {
            this.classList.remove('is-invalid');
        }
    });

    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }
});
