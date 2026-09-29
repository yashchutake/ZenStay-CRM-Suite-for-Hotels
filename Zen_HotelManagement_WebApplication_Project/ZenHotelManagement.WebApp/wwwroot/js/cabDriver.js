// Initialize tooltips and other UI enhancements
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Add smooth scrolling
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add hover effects for table rows
    document.querySelectorAll('table tbody tr').forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.style.transform = 'scale(1.01)';
            this.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)';
            this.style.transition = 'all 0.3s ease';
        });
        row.addEventListener('mouseleave', function() {
            this.style.transform = 'scale(1)';
            this.style.boxShadow = 'none';
        });
    });

    // Add loading animation for form submissions
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            if (this.checkValidity()) {
                document.body.style.cursor = 'wait';
            }
        });
    });

    // Add search input animations
    const searchInput = document.getElementById('SearchTerm');
    if (searchInput) {
        searchInput.addEventListener('focus', function() {
            this.parentElement.style.transform = 'scale(1.02)';
            this.parentElement.style.transition = 'all 0.3s ease';
        });

        searchInput.addEventListener('blur', function() {
            this.parentElement.style.transform = 'scale(1)';
        });

        // Add debounced search functionality
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.closest('form').submit();
            }, 500);
        });
    }

    // Add animation for status changes
    document.querySelectorAll('.badge').forEach(badge => {
        badge.addEventListener('mouseover', function() {
            this.style.transform = 'scale(1.1)';
            this.style.transition = 'all 0.3s ease';
        });

        badge.addEventListener('mouseout', function() {
            this.style.transform = 'scale(1)';
        });
    });

    // Add loading indicator
    function showLoading() {
        const loading = document.createElement('div');
        loading.className = 'loading';
        loading.innerHTML = '<div class="loading-spinner"></div>';
        document.body.appendChild(loading);
    }

    function hideLoading() {
        const loading = document.querySelector('.loading');
        if (loading) {
            loading.remove();
        }
    }

    // Add form submission animations
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function(e) {
            if (this.checkValidity()) {
                if (!this.classList.contains('filter-form')) {
                    showLoading();
                }
            }
        });
    });

    // Add button click effects
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('mousedown', function() {
            this.style.transform = 'scale(0.95)';
        });

        button.addEventListener('mouseup', function() {
            this.style.transform = 'scale(1)';
        });

        button.addEventListener('mouseleave', function() {
            this.style.transform = 'scale(1)';
        });
    });

    // Initialize any dropdowns
    document.querySelectorAll('.form-select').forEach(select => {
        select.addEventListener('change', function() {
            if (!this.closest('form').classList.contains('filter-form')) {
                this.closest('form').submit();
            }
        });
    });
});

// Status change confirmation
function confirmStatusChange(action, driverName) {
    const message = action === 'remove' 
        ? `Are you sure you want to mark ${driverName} as Past?` 
        : `Are you sure you want to restore ${driverName} as Current?`;
    
    if (confirm(message)) {
        // Show loading animation
        document.body.style.cursor = 'wait';
        return true;
    }
    return false;
}


