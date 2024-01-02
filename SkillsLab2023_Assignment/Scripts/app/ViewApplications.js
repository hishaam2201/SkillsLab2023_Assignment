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
        var fullName = `${pendingApplication.FirstName} ${pendingApplication.LastName }`
        const applicantNameCell = document.createElement('td')
        applicantNameCell.textContent = `${fullName}`
        row.appendChild(applicantNameCell)

        const trainingNameCell = document.createElement('td')
        trainingNameCell.textContent = pendingApplication.TrainingName
        row.appendChild(trainingNameCell)

        const trainingDepartmentCell = document.createElement('td')
        trainingDepartmentCell.textContent = pendingApplication.DepartmentName
        row.appendChild(trainingDepartmentCell)

        const applicationId = pendingApplication.ApplicationId
        const applicationStatusCell = document.createElement('td')
        applicationStatusCell.id = `applicationStatus_${applicationId}`;
        applicationStatusCell.textContent = pendingApplication.ApplicationStatus
        row.appendChild(applicationStatusCell)

        const viewDocumentButtonCell = createButton('view', 'View Document', () => {
            const firstName = pendingApplication.FirstName
            const lastName = pendingApplication.LastName
            fetchDataAndPopulateModal(firstName, lastName, applicationId)
        })

        const approveApplicationButtonCell = createButton('approve', 'Approve Application', () => {
            showSpinner()
            approveApplication(applicationId)
        });

        const declineApplicationButtonCell = createButton('decline', 'Decline Application', () => {
            submitDeclineModal(applicationId, fullName)
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
            button.setAttribute('data-bs-toggle', 'modal');
            button.setAttribute('data-bs-target', '#declineModal');
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
                    populateViewModal(data.Attachments);
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

function populateViewModal(attachmentInfoList) {
    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = '';

    attachmentInfoList.forEach(function (attachmentInfo) {
        var rowDiv = document.createElement('div');
        rowDiv.classList.add('row','p-2');

        var preRequisiteDiv = document.createElement('div');
        preRequisiteDiv.className = 'col-md-9';

        var preRequisiteParagraph = document.createElement('p');
        preRequisiteParagraph.innerHTML = `<strong>${attachmentInfo.PreRequisiteName}:</strong> ${attachmentInfo.PreRequisiteDescription}`;

        preRequisiteDiv.appendChild(preRequisiteParagraph);

        var buttonDiv = document.createElement('div');
        buttonDiv.classList.add("col-md-3", "text-center")

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
        .then(response => response.blob())
        .catch(() => {
            window.location.href = "/Common/InternalServerError"
        })
}

function approveApplication(applicationId) {
    fetch(`/EnrollmentProcess/ApproveApplication?applicationId=${applicationId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success("Application approved!", "Approved", {
                    timeOut: 1200,
                    progressBar: true
                })

                setTimeout(() => {
                    hideSpinner();
                    window.location.reload()
                }, 1300)
            }
            else {
                toastr.error("Could not approve application", "Error", {
                    timeOut: 5000,
                    progressBar: true
                })
            }
        })
        .catch(() => {
            hideSpinner();
            window.location.href = "/Common/InternalServerError"
        })
}

function submitDeclineModal(applicationId, fullName) {
    var declineModal = document.getElementById('declineModal')
    if (declineModal) {
        const declineModalTitle = document.getElementById("declineModalTitle")
        declineModalTitle.textContent = `Decline reason to ${fullName}`
    }
    const declineButton = document.getElementById("declineButton");
    declineButton.addEventListener('click', function () {
        const declineMessage = document.getElementById('decline-message-text').value;
        showSpinner()
        declineApplication(applicationId, declineMessage)
    })
}

function declineApplication(applicationId, message) {
    var formData = new FormData()
    formData.append('applicationId', applicationId)
    formData.append('message', message)

    fetch(`/EnrollmentProcess/DeclineApplication`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success("Declined message sent successfully!", "Message sent", {
                    timeOut: 1200,
                    progressBar: true
                })

                setTimeout(() => {
                    hideSpinner();
                    window.location.reload()
                }, 1300)
            }
            else {
                toastr.error("Could not send declined reason", "Error", {
                    timeOut: 5000,
                    progressBar: true
                })
            }
        })
        .catch(() => {
            hideSpinner();
            window.location.href = "/Common/InternalServerError"
        })
}

function showSpinner() {
    const spinnerWrapper = document.getElementById('emailSpinner');
    spinnerWrapper.style.display = 'flex';
}

function hideSpinner() {
    const spinnerWrapper = document.getElementById('emailSpinner');
    spinnerWrapper.style.opacity = '0';
    spinnerWrapper.style.display = 'none';
}