var toastMixin = Swal.mixin({
    toast: true,
    icon: 'success',
    title: 'General Title',
    animation: false,
    position: 'top-right',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});

var successMessageElement = document.getElementById('SuccessMessage');
var errorMessageElement = document.getElementById('ErrorMessage');

var successMessage, errorMessage;

if (successMessageElement) {
    successMessage = successMessageElement.textContent;
}

if (errorMessageElement) {
    errorMessage = errorMessageElement.textContent;
}

document.addEventListener('DOMContentLoaded', function () {
    
    var toastData = document.getElementById('toast-data');

    // Get the toast type from the data attribute
    if (toastData && toastData.hasAttribute && typeof toastData.hasAttribute === 'function' && toastData.hasAttribute('data-toast-type')) {
        var toastType = toastData.getAttribute('data-toast-type');
    } else {
        return;
    }

    // Display the appropriate toast based on the toastType
    if (toastType === 'success') {
        toastMixin.fire({
            title: successMessage,
            icon: 'success'
        });
    } else if (toastType === 'error') {
        toastMixin.fire({
            title: errorMessage,
            icon: 'error'
        });
    } else if (toastType === 'info') {
        toastMixin.fire({
            title: 'Info',
            icon: 'info'
        });
    }
});