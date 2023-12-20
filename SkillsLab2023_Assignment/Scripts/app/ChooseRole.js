const form = document.getElementById('chooseRoleForm');
form.addEventListener('submit', function (e) {
    e.preventDefault()

    var selectedRole = document.querySelector('input[name="role"]:checked')
    if (selectedRole) {
        var selectedRoleValue = selectedRole.value
        fetch('/Account/ChooseRole', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ selectedRole: selectedRoleValue })
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
    else {
        toastr.error(`${selectedRoleValue}`, 'Error', {
            timeOut: 1500,
            progressBar: true
        })
    }
})

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