
const registrationForm = document.getElementById('registrationForm')

registrationForm.addEventListener('submit', event => {
    if (!registrationForm.checkValidity()) {
        event.preventDefault()
        event.stopPropagation()
    }
    else {
        submitRegistrationForm();
    }

    registrationForm.classList.add('was-validated')

}, false)

function submitRegistrationForm() {

    var firstName = document.getElementById("firstName").value.trim()
    var lastName = document.getElementById("lastName").value.trim()
    var mobileNumber = document.getElementById("mobileNumber").value.trim()
    var nationalIdentityCard = document.getElementById("nationalIdentityCard").value.trim()
    var managerName = document.getElementById("managerName").value.trim()
    var department = document.getElementById("department").value.trim()
    var email = document.getElementById("email").value.trim()
    var password = document.getElementById("password").value.trim()

    var formData = new FormData()
    formData.append("FirstName", firstName)
    formData.append("LastName", lastName)
    formData.append("MobileNumber", mobileNumber)
    formData.append("NationalIdentityCard", nationalIdentityCard)
    formData.append("ManagerName", managerName)
    formData.append("DepartmentId", department)
    formData.append("Email", email)
    formData.append("Password", password)

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