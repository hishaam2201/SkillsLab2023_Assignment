function submitRegistration() {

    var formData = new FormData()
    formData.append("FirstName", document.getElementById("firstName").value)
    formData.append("LastName", document.getElementById("lastName").value)
    formData.append("MobileNumber", document.getElementById("mobileNumber").value)
    formData.append("NationalIdentityCard", document.getElementById("nic").value)
    formData.append("ManagerName", document.getElementById("managerName").value)
    formData.append("DepartmentId", document.getElementById("department").value)
    formData.append("Email", document.getElementById("email").value)
    formData.append("Password", document.getElementById("password").value)

    fetch('/Account/Register', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                return response.json()
            }
            else {
                console.error('Response status: ', response.status)
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