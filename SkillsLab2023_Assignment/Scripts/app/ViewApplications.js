(function () {
    const applicationsTable = document.getElementById('applicationsTable')
    new DataTable(applicationsTable, {
        responsive: true,
        paging: true,
        ordering: false,
        pageLength: 7,
        language: {
            lengthMenu:
                '<span>Show: <select class="form-select mb-2">' +
                '<option value="7">7</option>' +
                '<option value="15">15</option>' +
                '<option value="25">25</option>' +
                '<option value="-1">All</option>' +
                '</select> entries</span>',
        },
        drawCallback: function () {
            addButtonClickListener('view-btn', function (button, applicationId) {
                disableButton(button)
                var employeeName = button.getAttribute('data-employee-name')
                fetchDataAndPopulateModal(button, employeeName, applicationId)
            })
            addButtonClickListener('approve-btn', function (button, applicationId) {
                disableButton(button)
                approveApplication(button, applicationId)
            });

            addButtonClickListener('decline-btn', function (button, applicationId) {
                var employeeName = button.getAttribute('data-employee-name')
                submitDeclineModal(applicationId, employeeName)
            });
        }
    });
})()


function fetchDataAndPopulateModal(button, employeeName, applicationId) {
    var formData = new FormData()
    formData.append('applicationId', applicationId)
    fetch(`/Application/ViewDocuments`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            const modal = document.getElementById("viewDocModal");
            const modalTitle = modal.querySelector(".modal-title");

            modalTitle.textContent = `Documents submitted by ${employeeName}`

            if (data.success) {
                if (data.Attachments && data.Attachments.length > 0) {
                    populateViewModal(data.Attachments);
                }
            }
            else {
                var modalBody = document.getElementById('modalBody');
                modalBody.innerHTML = '<p>' + data.message + '</p>'
            }
        })
        .catch(() => {
            window.location.href = "/Common/InternalServerError"
        })
        .finally(() => {
            setTimeout(() => {
                enableButton(button)
            }, 500)
        })
}

function populateViewModal(attachmentInfoList) {
    var modalBody = document.getElementById("modalBody")
    modalBody.innerHTML = '';

    attachmentInfoList.forEach(function (attachmentInfo) {
        var containerDiv = document.createElement('div');
        containerDiv.className = 'container'
        var rowDiv = document.createElement('div')
        rowDiv.classList.add('row', 'align-items-center', 'mb-3')

        var preRequisiteDiv = document.createElement('div');
        preRequisiteDiv.classList.add('col-md-11')

        var preRequisiteParagraph = document.createElement('p');
        preRequisiteParagraph.classList.add('align-self-center', 'mb-0')
        preRequisiteParagraph.innerHTML = `<strong>${attachmentInfo.PreRequisiteName}:</strong> ${attachmentInfo.PreRequisiteDescription}`;

        preRequisiteDiv.appendChild(preRequisiteParagraph);

        var buttonDiv = document.createElement('div');
        buttonDiv.classList.add("col-md-1", "text-center")

        var viewDocumentLink = document.createElement('a');
        viewDocumentLink.href = `/Application/DownloadAttachment?attachmentId=${attachmentInfo.AttachmentId}`; // Downloading file
        viewDocumentLink.className = 'btn btn-primary';
        viewDocumentLink.innerHTML = '<i class="fa-solid fa-download"></i>';
        buttonDiv.appendChild(viewDocumentLink);

        rowDiv.appendChild(preRequisiteDiv);
        rowDiv.appendChild(buttonDiv);
        containerDiv.appendChild(rowDiv)

        modalBody.appendChild(containerDiv);
    })
}

function approveApplication(button, applicationId) {
    var formData = new FormData()
    formData.append('applicationId', applicationId)
    fetch(`/Application/ApproveApplication`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success(`${data.message}`, "Approved", {
                    timeOut: 1200,
                    progressBar: true
                })
            }
            else {
                toastr.error(`${data.message}`, "Error")
            }
        })
        .catch(() => {
            window.location.href = "/Common/InternalServerError"
        })
        .finally(() => {
            setTimeout(() => {
                enableButton(button)
                window.location.reload()
            }, 2000)
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
        declineButton.disabled = true
        const declineTextArea = document.getElementById('decline-message-text')
        declineApplication(applicationId, declineTextArea)
    })
}

function declineApplication(applicationId, declineTextArea) {
    var formData = new FormData()
    formData.append('applicationId', applicationId)
    formData.append('message', declineTextArea.value)

    fetch(`/Application/DeclineApplication`, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success(`${data.message}`, "Rejected", {
                    timeOut: 1200,
                    progressBar: true
                })
            }
            else {
                toastr.error(`${data.message}`, "Error")
            }
        })
        .catch(() => {
            window.location.href = "/Common/InternalServerError"
        })
        .finally(() => {
            setTimeout(() => {
                window.location.reload()
            }, 1500)
        })
}

function enableButton(button) {
    if (button) {
        toggleElementVisibility(button, '.spinner', false)
        toggleElementVisibility(button, '.icon', true)
        button.disabled = false;
    }
}

function disableButton(button) {
    if (button) {
        toggleElementVisibility(button, '.spinner', true)
        toggleElementVisibility(button, '.icon', false)
        button.disabled = true
    }
}

function toggleElementVisibility(parentElement, selector, isVisible) {
    var element = parentElement.querySelector(selector);
    if (element) {
        element.style.display = isVisible ? 'inline-block' : 'none';
    }
}

function addButtonClickListener(className, action) {
    var buttons = document.querySelectorAll(`.${className}`);
    buttons.forEach(function (button) {
        button.removeEventListener('click', button.clickHandler);
        button.clickHandler = function () {
            var applicationId = button.getAttribute('data-application-id');
            action(button, applicationId);
        };
        button.addEventListener('click', button.clickHandler);
    });
}