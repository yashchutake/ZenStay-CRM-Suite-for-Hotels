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

function showDeleteConfirmation(action, driverName) {
    return Swal.fire({
        title: action === 'remove'
            ? `Mark ${driverName} as past?`
            : `Restore ${driverName} as Current?`,
        text: action === 'remove'
            ? "This will mark the cab driver as past"
            : "This will restore the cab driver as current",
        icon: action === 'remove' ? 'warning' : 'question',
        showCancelButton: true,
        confirmButtonColor: action === 'remove' ? '#d33' : '#3085d6',
        cancelButtonColor: '#6c757d',
        confirmButtonText: action === 'remove' ? 'Yes, mark as past' : 'Yes, restore',
        cancelButtonText: 'Cancel',
        customClass: {
            popup: 'animate__animated animate__fadeInDown'
        }
    });
}

