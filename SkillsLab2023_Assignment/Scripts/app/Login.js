
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
        .then(response => {
            if (response.ok) {
                return response.json()
            }
            else {
                console.error('Error: ', response.status)
            }
        })
        .then(data => {
            if (data.success) {
                displayMessageToUser("success", data.message)
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 1500)
            }
            else {
                displayMessageToUser("danger", data.message)
            }
        })
        .catch(error => {
            console.error('Error: ', error)
        })
}
function displayMessageToUser(category, message) {
    let messageContainer = document.getElementById('message-container')
    messageContainer.style.display = 'block'
    messageContainer.innerHTML =
        `<div class="alert alert-${category} alert-dismissible fade show mt-4" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>`
}