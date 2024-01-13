(function () {
    const selectedUsersTable = document.getElementById('selectedUsers')
    new DataTable(selectedUsersTable, {
        responsive: true,
        paging: true,
        ordering: false
    })

    var exportButton = document.getElementById('exportBtn');
    exportButton.addEventListener('click', function () {
        downloadExcelFile()
    })
})()

function downloadExcelFile() {
    var trainingId = document.getElementById('trainingId').value
    var formData = new FormData()
    formData.append('trainingId', trainingId)
    fetch(`/EnrollmentProcess/DownloadSelectedUsers`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            var fileName = response.headers.get('content-disposition').split('filename=')[1]

            return response.blob().then(blob => {
                const downloadLink = document.createElement('a')
                downloadLink.href = URL.createObjectURL(blob)

                downloadLink.download = fileName || 'ExportedUsers'

                document.body.appendChild(downloadLink)
                downloadLink.click()
                document.body.removeChild(downloadLink)
            })
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
}