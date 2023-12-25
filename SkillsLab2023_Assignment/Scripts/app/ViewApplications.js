(function () {
    fetch('/EnrollmentProcess/ApproveApplication', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                if (data.success) {
                    if (data.PendingApplications && data.PendingApplications.length > 0) {
                        console.log(data.PendingApplications)
                        populateApplicationTable(data.PendingApplications)
                    }
                    else {
                        toastr.warning("No pending applications found", {
                            timeOut: 5000,
                            progressBar: true
                        })
                    }
                }
            }
            else {
                toastr.error("Applications could not be fetched", "Error", {
                    timeOut: 5000,
                    progressBar: true
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
})()

function populateApplicationTable(pendingApplications) {
    const tableBody = document.getElementById('tableBody')

    pendingApplications.forEach(pendingApplication => {
        const row = document.createElement('tr')

        const applicantNameCell = document.createElement('td')
        applicantNameCell.textContent = `${pendingApplication.FirstName} ${pendingApplication.LastName}`
        row.appendChild(applicantNameCell)

        const trainingNameCell = document.createElement('td')
        trainingNameCell.textContent = pendingApplication.TrainingName
        row.appendChild(trainingNameCell)

        const applicationStatusCell = document.createElement('td')
        applicationStatusCell.textContent = pendingApplication.ApplicationStatus
        row.appendChild(applicationStatusCell)

        const viewDocumentButtonCell = document.createElement('td')
        const viewDocumentButton = document.createElement('button')
        viewDocumentButton.classList.add('btn');
        viewDocumentButton.classList.add('btn-secondary');
        viewDocumentButton.textContent = 'View Document';
        viewDocumentButton.addEventListener('click', () => {
            alert("View Document Button Clicked")
        })
        viewDocumentButtonCell.appendChild(viewDocumentButton)
        row.appendChild(viewDocumentButtonCell)

        const approveApplicationButtonCell = document.createElement('td')
        const approveApplicationButton = document.createElement('button')
        approveApplicationButton.classList.add('btn');
        approveApplicationButton.classList.add('btn-outline-success');
        approveApplicationButton.textContent = 'Approve Application';
        approveApplicationButton.addEventListener('click', () => {
            alert("Approve Application Button Clicked")
        })
        approveApplicationButtonCell.appendChild(approveApplicationButton)
        row.appendChild(approveApplicationButtonCell)

        const declineApplicationButtonCell = document.createElement('td')
        const declineApplicationButton = document.createElement('button')
        declineApplicationButton.classList.add('btn');
        declineApplicationButton.classList.add('btn-outline-danger');
        declineApplicationButton.textContent = 'Decline Application';
        declineApplicationButton.addEventListener('click', () => {
            alert("Decline Application Button Clicked")
        })
        declineApplicationButtonCell.appendChild(declineApplicationButton)
        row.appendChild(declineApplicationButtonCell)

        tableBody.appendChild(row)
    })
}
