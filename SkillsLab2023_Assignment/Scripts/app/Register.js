(function () {
    getAllDepartments()
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
})()

function getAllDepartments() {
    fetch('/Account/GetAllDepartments', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                var departments = data.departments

                var departmentSelectElement = document.getElementById('department')
                departmentSelectElement.innerHTML = '<option selected disabled value="">Select your Department</option>'

                departments.forEach(function (department) {
                    var option = document.createElement('option')
                    option.value = department.Id
                    option.text = department.DepartmentName
                    departmentSelectElement.appendChild(option)
                })

                departmentSelectElement.addEventListener('change', function () {
                    var selectedDepartmentId = departmentSelectElement.value
                    getAllManagersFromDepartment(selectedDepartmentId)
                })
            }
        })
        .catch(() => {
            toastr.error("Error fetching Departments", "Error", {
                closeButton: true
            })
        })
}

function getAllManagersFromDepartment(selectedDepartmentId) {
    fetch(`/Account/GetAllManagersFromDepartment?departmentId=${selectedDepartmentId}`, {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            var managerSelectElement = document.getElementById('manager')
            managerSelectElement.innerHTML = '<option selected disabled value="">Select your Manager</option>'

            if (data.success && data.managers.length > 0) {
                var managers = data.managers;

                managers.forEach(function (manager) {
                    var option = document.createElement('option')
                    option.value = manager.Id
                    option.text = `${manager.FirstName} ${manager.LastName}`
                    managerSelectElement.appendChild(option)
                })
            }
            else {
                toastr.warning("No managers available for the selected department", "No Managers Found", {
                    progressBar: true,
                    timeOut: 3000
                })
            }
        })
        .catch(() => {
            toastr.error("Error fetching Managers", "Error", {
                closeButton: true
            })
        })
}

function submitRegistrationForm() {

    var firstName = document.getElementById("firstName").value.trim()
    var lastName = document.getElementById("lastName").value.trim()
    var mobileNumber = document.getElementById("mobileNumber").value.trim()
    var nationalIdentityCard = document.getElementById("nationalIdentityCard").value.trim()
    var department = document.getElementById("department").value.trim()
    var manager = document.getElementById("manager").value.trim()
    var email = document.getElementById("email").value.trim()

    var password = document.getElementById("password")
    var confirmPassword = document.getElementById("confirmPassword")
    if (password.value.trim() != confirmPassword.value.trim()) {
        toastr.error("Passwords do not match", "Error", {
            closeButton: true
        })
        return false;
    }

    var formData = new FormData()
    formData.append("FirstName", firstName)
    formData.append("LastName", lastName)
    formData.append("MobileNumber", mobileNumber)
    formData.append("NationalIdentityCard", nationalIdentityCard)
    formData.append("ManagerId", manager)
    formData.append("DepartmentId", department)
    formData.append("Email", email)
    formData.append("Password", password)

    fetch('/Account/Register', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                displayToastToUser('success', data.message)
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 1500)
            }
            else {
                displayToastToUser('error', data.message)
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
}

function displayToastToUser(toastColor, message) {
    if (toastColor === 'success') {
        toastr.success(`${message}`, "Success", {
            timeOut: 1000,
            progressBar: true
        })
    }
    else {
        toastr.error(`${message}`, "Error", {
            closeButton: true
        })
    }
}