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
    var currentUrl = window.location.href
    var url = new URL(currentUrl)
    var trainingId = url.searchParams.get("trainingId")
    fetch(`/EnrollmentProcess/DownloadSelectedUsers?trainingId=${trainingId}`, {
        method: 'POST'
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