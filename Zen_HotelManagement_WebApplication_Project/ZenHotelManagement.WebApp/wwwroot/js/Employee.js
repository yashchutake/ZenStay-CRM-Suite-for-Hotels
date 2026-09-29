function showNotification(type, message) {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });

    const icons = {
        success: 'success',
        info: 'info',
        warning: 'warning',
        error: 'error'
    };

    Toast.fire({
        icon: type.toLowerCase(),
        title: message,
        customClass: {
            popup: 'animate__animated animate__fadeInDown'
        }
    });
}

function showDeleteConfirmation(action, Name) {
    return Swal.fire({
        title: action === 'remove'
            ? `Mark ${Name} as inactive?`
            : `Restore ${Name} as active?`,
        text: action === 'remove'
            ? "This will mark the employee as inactive"
            : "This will restore the employee as active",
        icon: action === 'remove' ? 'warning' : 'question',
        showCancelButton: true,
        confirmButtonColor: action === 'remove' ? '#d33' : '#3085d6',
        cancelButtonColor: '#6c757d',
        confirmButtonText: action === 'remove' ? 'Yes, mark as inactive' : 'Yes, active',
        cancelButtonText: 'Cancel',
        customClass: {
            popup: 'animate__animated animate__fadeInDown'
        }
    });
}

