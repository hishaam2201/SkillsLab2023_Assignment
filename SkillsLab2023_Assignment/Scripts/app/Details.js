
const form = document.getElementById('uploadForm')
form.addEventListener('submit', event => {
    if (!form.checkValidity()) {
        event.preventDefault()
        event.stopPropagation()
    }
    else {
        // submit form
    }

    form.classList.add('was-validated')
}, false)


function displayToastToUser(toastColor, message) {
    if (toastColor === 'success') {
        toastr.success(`${message}`, "Success", {
            timeOut: 1500,
            progressBar: true
        })
    }
    else {
        toastr.error(`${message}`, "Error", {
            timeOut: 5000,
            progressBar: true
        })
    }
}