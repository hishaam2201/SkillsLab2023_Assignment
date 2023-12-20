const form = document.getElementById('chooseRoleForm');
form.addEventListener('submit', function (e) {
    e.preventDefault()

    var selectedRole = document.querySelector('input[name="role"]:checked')
    if (selectedRole) {
        var selectedRoleValue = selectedRole.value
        toastr.success(`${selectedRoleValue}`, 'Success', {
            timeOut: 1500,
            progressBar: true
        })
    }
    else {
        toastr.error(`${selectedRoleValue}`, 'Error', {
            timeOut: 1500,
            progressBar: true
        })
    }
})