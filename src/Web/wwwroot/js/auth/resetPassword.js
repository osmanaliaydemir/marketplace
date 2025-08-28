// Reset Password Form Validation and Password Visibility Toggle
document.addEventListener('DOMContentLoaded', function() {
    const togglePassword = document.getElementById('togglePassword');
    const toggleConfirmPassword = document.getElementById('toggleConfirmPassword');
    const newPassword = document.getElementById('newPassword');
    const confirmPassword = document.getElementById('confirmPassword');
    const form = document.querySelector('form');

    if (!form) return;

    // Password visibility toggle
    if (togglePassword && newPassword) {
        togglePassword.addEventListener('click', function() {
            const type = newPassword.getAttribute('type') === 'password' ? 'text' : 'password';
            newPassword.setAttribute('type', type);
            this.querySelector('i').classList.toggle('fa-eye');
            this.querySelector('i').classList.toggle('fa-eye-slash');
        });
    }

    if (toggleConfirmPassword && confirmPassword) {
        toggleConfirmPassword.addEventListener('click', function() {
            const type = confirmPassword.getAttribute('type') === 'password' ? 'text' : 'password';
            confirmPassword.setAttribute('type', type);
            this.querySelector('i').classList.toggle('fa-eye');
            this.querySelector('i').classList.toggle('fa-eye-slash');
        });
    }

    // Form validation
    form.addEventListener('submit', function(e) {
        if (!newPassword || !confirmPassword) return;

        const newPasswordValue = newPassword.value;
        const confirmPasswordValue = confirmPassword.value;

        // Clear previous validation
        newPassword.classList.remove('is-invalid');
        confirmPassword.classList.remove('is-invalid');

        let isValid = true;

        // Check password length
        if (newPasswordValue.length < 8) {
            newPassword.classList.add('is-invalid');
            isValid = false;
        }

        // Check password complexity
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/;
        if (!passwordRegex.test(newPasswordValue)) {
            newPassword.classList.add('is-invalid');
            isValid = false;
        }

        // Check password confirmation
        if (newPasswordValue !== confirmPasswordValue) {
            confirmPassword.classList.add('is-invalid');
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault();
        }
    });
});
