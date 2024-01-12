const form = document.getElementById('chooseRoleForm');
form.addEventListener('submit', function (e) {
    e.preventDefault()

    var selectedRole = document.querySelector('input[name="role"]:checked')
    if (selectedRole) {
        var selectedRoleValue = selectedRole.value
        var formData = new FormData()
        formData.append('selectedRole', selectedRoleValue)
        fetch('/Account/ChooseRole', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    displayToastToUser('success', data.message)
                    setTimeout(() => {
                        window.location.href = data.redirectUrl
                    }, 700)
                }
                else {
                    displayToastToUser('error', data.message)
                }
            })
            .catch(() => {
                window.location.href = '/Common/InternalServerError'
            })
    }
    else {
        toastr.error(`${selectedRoleValue}`, 'Error', {
            closeButton: true
        })
    }
})

function displayToastToUser(toastColor, message) {
    if (toastColor === 'success') {
        toastr.success(`${message}`, "Success", {
            timeOut: 700,
            progressBar: true
        })
    }
    else {
        toastr.error(`${message}`, "Error", {
            closeButton: true
        })
    }
}