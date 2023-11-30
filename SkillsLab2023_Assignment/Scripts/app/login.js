function submitForm(event) {

    event.preventDefault();
    var formData = new FormData()

    formData.append("Email", document.getElementById("email").value)
    formData.append("Password", document.getElementById("password").value)

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
                console.error(data.message)
            }
        })
        .catch(error => {
            console.error('Error: ', error)
        })
}