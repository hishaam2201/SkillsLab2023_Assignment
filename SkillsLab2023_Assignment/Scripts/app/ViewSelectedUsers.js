(function () {
    const selectedUsersTable = document.getElementById('selectedUsers')
    new DataTable(selectedUsersTable, {
        responsive: true,
        paging: true,
        ordering: false
    })
})()