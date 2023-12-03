function submitLoginForm(event) {

    event.preventDefault();
    var formData = new FormData()

    var email = document.getElementById("email").value
    var password = document.getElementById("password").value

    console.log(email)
    console.log(password)

    formData.append("Email", email)
    formData.append("Password", password)

    console.log(formData)

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
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 1500)
            }
            else {
                let errorContainer = document.getElementById('error-container')
                errorContainer.style.display = 'block'
                errorContainer.innerHTML =
                    `<div class="alert alert-danger alert-dismissible fade show mt-4" role="alert">
                    ${data.message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>`
            }
        })
        .catch(error => {
            console.error('Error: ', error)
        })
}