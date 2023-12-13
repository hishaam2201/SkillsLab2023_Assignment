
const loginForm = document.getElementById('loginForm')

loginForm.addEventListener('submit', event => {
    if (!loginForm.checkValidity()) {
        event.preventDefault()
        event.stopPropagation()
    }
    else {
        submitLoginForm();
    }
    loginForm.classList.add('was-validated')

}, false)

function submitLoginForm() {
    var email = document.getElementById("email");
    var password = document.getElementById("password");

    var formData = new FormData()
    formData.append("Email", email.value.trim())
    formData.append("Password", password.value.trim())

    fetch('/Account/Login', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                displayToastToUser('success', data.message)
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 3000)
            }
            else {
                displayToastToUser('error', data.message)
            }
        })
        .catch(error => {
            window.location.href = '/Common/InternalServerError'
        })
}
function displayToastToUser(toastColor, message) {
    if (toastColor === 'success') {
        toastr.success(`${message}`, "Success", {
            timeOut: 2500,
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