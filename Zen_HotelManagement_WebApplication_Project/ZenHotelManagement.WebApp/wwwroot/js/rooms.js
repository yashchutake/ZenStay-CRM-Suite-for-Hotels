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

function showDeleteConfirmation(action, RoomNo) {
    return Swal.fire({
        title: action === 'remove'
            ? `Mark ${RoomNo} as inactive?`
            : `Restore ${RoomNo} as active?`,
        text: action === 'remove'
            ? "This will mark the room as inactive"
            : "This will restore the room as active",
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

