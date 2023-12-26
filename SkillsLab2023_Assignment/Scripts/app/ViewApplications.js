(function () {
    fetch('/EnrollmentProcess/ViewApplications', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                if (data.PendingApplications && data.PendingApplications.length > 0) {
                    populateApplicationTable(data.PendingApplications)
                }
            }
            else {
                document.getElementById("dashboardContainer").innerHTML = '<h1>No pending applications found</h1>'
                toastr.warning("No pending applications found", {
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

        const trainingDepartmentCell = document.createElement('td')
        trainingDepartmentCell.textContent = pendingApplication.DepartmentName
        row.appendChild(trainingDepartmentCell)

        const applicationStatusCell = document.createElement('td')
        applicationStatusCell.textContent = pendingApplication.ApplicationStatus
        row.appendChild(applicationStatusCell)

        const viewDocumentButtonCell = createButton('view', 'View Document', () => {
            const firstName = pendingApplication.FirstName
            const lastName = pendingApplication.LastName
            const applicationId = pendingApplication.ApplicationId
            fetchDataAndPopulateModal(firstName, lastName, applicationId)
        })

        const approveApplicationButtonCell = createButton('approve', 'Approve Application', () => {
            alert('Approve Application Button Clicked');
        });

        const declineApplicationButtonCell = createButton('decline', 'Decline Application', () => {
            alert('Decline Application Button Clicked');
        });

        row.appendChild(viewDocumentButtonCell)
        row.appendChild(approveApplicationButtonCell)
        row.appendChild(declineApplicationButtonCell)

        tableBody.appendChild(row)
    })
}

function createButton(type, text, clickHandler) {
    const buttonCell = document.createElement('td');
    const button = document.createElement('button');
    button.type = 'button';
    button.classList.add('btn');

    switch (type) {
        case 'view':
            button.classList.add('btn-secondary');
            button.setAttribute('data-bs-toggle', 'modal');
            button.setAttribute('data-bs-target', '#viewDocModal');
            button.addEventListener('click', clickHandler);
            break;
        case 'approve':
            button.classList.add('btn-outline-success');
            button.addEventListener('click', clickHandler);
            break;
        case 'decline':
            button.classList.add('btn-outline-danger');
            button.addEventListener('click', clickHandler);
            break;
        default:
            break;
    }

    button.textContent = text;
    buttonCell.appendChild(button);
    return buttonCell;
}

function fetchDataAndPopulateModal(firstName, lastName, applicationId) {
    fetch(`/EnrollmentProcess/ViewDocuments?applicationId=${applicationId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            const modal = document.getElementById("viewDocModal");
            const modalTitle = modal.querySelector(".modal-title");

            modalTitle.textContent = `Documents submitted by ${firstName} ${lastName}`

            if (data.success) {
                if (data.Attachments && data.Attachments.length > 0) {
                    populateModal(data.Attachments);
                }
                else {
                    toastr.warning("No attachments found", {
                        timeOut: 5000,
                        progressBar: true
                    })
                }
            }
            else {
                toastr.error("Documents could not be fetched", "Error", {
                    timeOut: 5000,
                    progressBar: true
                })
            }
        })
        .catch(() => {
            window.location.href = "/Common/InternalServerError"
        })
}

function populateModal(attachmentInfoList) {
    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = '';

    attachmentInfoList.forEach(function (attachmentInfo) {
        var rowDiv = document.createElement('div');
        rowDiv.className = 'row';

        var preRequisiteDiv = document.createElement('div');
        preRequisiteDiv.className = 'col-md-8';

        var preRequisiteParagraph = document.createElement('p');
        preRequisiteParagraph.innerHTML = '<strong>Pre-Requisite:</strong> ' + attachmentInfo.PreRequisiteDescription;

        preRequisiteDiv.appendChild(preRequisiteParagraph);

        var buttonDiv = document.createElement('div');
        buttonDiv.classList.add("col-md-4", "text-center")

        var viewButton = document.createElement('a');
        viewButton.href = `/EnrollmentProcess/DownloadAttachment?attachmentId=${attachmentInfo.AttachmentId}`;
        viewButton.className = 'btn btn-primary';
        viewButton.textContent = 'View Document';
        buttonDiv.appendChild(viewButton);

        rowDiv.appendChild(preRequisiteDiv);
        rowDiv.appendChild(buttonDiv);

        modalBody.appendChild(rowDiv);
    })
}

function downloadAttachment(attachmentId) {
    fetch(`/EnrollmentProcess/DownloadAttachment?attachmentId=${attachmentId}`, {
        method: 'GET'
    })
        .then(response => response.json())
        .catch(() => {
            window.location.href = "/Common/InternalServerError"
        })
}
