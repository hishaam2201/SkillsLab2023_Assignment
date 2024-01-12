// TODO: Break code into different scripts of Add Update and Delete
(function () {
    var addSlimSelect = new SlimSelect({
        select: document.getElementById('addTrainingAllPreRequisites'),
        settings: {
            placeholderText: 'Add Pre-Requisites',
            allowDeselect: true,
            searchHighlight: true,
            closeOnSelect: false,
            hideSelected: true
        }
    })

    var editSlimSelect = new SlimSelect({
        select: document.getElementById('allPreRequisites'),
        settings: {
            placeholderText: 'Edit Pre-Requisites',
            allowDeselect: true,
            searchHighlight: true,
            closeOnSelect: false,
            hideSelected: true
        }
    })

    var allTrainingsTable = document.getElementById('allTrainings');
    new DataTable(allTrainingsTable, {
        responsive: true,
        paging: true,
        order: [[0, 'desc']],
        columnDefs: [
            {
                "targets": [2, 5, 6, 7, 8],
                "orderable": false,
            }
        ],
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
            addButtonClickListener('selection-btn', function (button, trainingId) {
                disableButton(button)
                performSelectionForTraining(trainingId, button)
                setTimeout(() => {
                    document.getElementById('trainingIdInput').value = trainingId
                    document.getElementById('selectionForm').submit()
                }, 2000)
            })
            addButtonClickListener('edit-btn', function (button, trainingId) {
                disableButton(button)
                setTimeout(() => {
                    fetchTrainingDetails(editSlimSelect, trainingId, button)
                }, 1000)
            });

            addButtonClickListener('delete-btn', function (button, trainingId) {
                disableButton(button)
                setTimeout(() => {
                    deleteTraining(button, trainingId)
                }, 1500)
            });
        }
    });

    // Validation on Edit Training Form
    const editTrainingForm = document.getElementById('editTrainingForm')
    editTrainingForm.addEventListener('submit', event => {
        if (!editTrainingForm.checkValidity()) {
            event.preventDefault()
            event.stopPropagation()
        }
        else {
            document.getElementById('saveChangesBtn').disabled = true
            submitEditTrainingForm(editSlimSelect)
        }
        editTrainingForm.classList.add('was-validated')

    }, false)

    // Populating departments and pre-requisites Insert training select
    const departmentPromise = fetchDepartments();
    const preRequisitesPromise = fetchPreRequisites()
    Promise.all([departmentPromise, preRequisitesPromise])
        .then(([departmentsData, preRequisitesData]) => {
            populateDepartmentOptions(document.getElementById('addTrainingDepartments'), departmentsData, [], false)
            populatePreRequisitesOptions(addSlimSelect, preRequisitesData, [], false)
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })

    // Rendering Insert Training Form
    var addTrainingBtn = document.getElementById('addTrainingBtn');
    var addTrainingIcon = document.getElementById('addTrainingIcon');
    var collapsed = false;
    var addTrainingForm = document.getElementById('addTrainingForm')
    addTrainingBtn.addEventListener('click', function () {
        collapsed = !collapsed;
        addTrainingIcon.style.transform = collapsed ? 'rotate(45deg)' : 'rotate(0deg)';

        // Validation on dates
        const applicationDeadlineInput = document.getElementById('addTrainingApplicationDeadline')
        const trainingStartDate = document.getElementById('addTrainingStartDate');
        const currentDate = new Date();
        const formattedCurrentDate = currentDate.toISOString().split('T')[0];
        // Limit deadline, initial call
        trainingStartDate.addEventListener('change', function () {
            updateDeadlineMinMaxLimits(applicationDeadlineInput, formattedCurrentDate, trainingStartDate.value)
        });

        // Reset Bootstrap validation when button is clicked to close the collapse
        addTrainingForm = document.getElementById('addTrainingForm');
        addTrainingForm.classList.remove('was-validated');
        clearForm(addTrainingForm)
    });

    // Validation on insert training form
    addTrainingForm.addEventListener('submit', event => {
        if (!addTrainingForm.checkValidity()) {
            event.preventDefault()
            event.stopPropagation()
        }
        else {
            disableButton(document.getElementById('nextBtn'))
            submitAddTrainingForm(addSlimSelect)
        }
        addTrainingForm.classList.add('was-validated')

    }, false)
})()

function performSelectionForTraining(trainingId, button) {
    fetch(`/EnrollmentProcess/PerformManualSelectionProcess?trainingId=${trainingId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success(`${data.message}`, "Selection successful", {
                    timeOut: 1000,
                    progressBar: true
                })
            }
            else {
                toastr.warning(`${data.message}`, "Users not found for selection", {
                    timeOut: 2000,
                    progressBar: true
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
        .finally(() => {
            setTimeout(() => {
                enableButton(button)
            }, 1500)
        })
}

function submitAddTrainingForm(addSlimSelect) {
    const trainingName = document.getElementById('addTrainingName').value
    const description = document.getElementById('addTrainingDescription').value
    const applicationDeadline = document.getElementById('addTrainingApplicationDeadline').value
    const capacity = document.getElementById('addTrainingCapacity').value

    const trainingStartDate = document.getElementById('addTrainingStartDate').value;
    const trainingStartTime = document.getElementById('addTrainingStartTime').value;
    const trainingStartDateTime = `${trainingStartDate} ${trainingStartTime}`

    const departmentId = document.getElementById('addTrainingDepartments').value
    const preRequisiteIds = addSlimSelect.getSelected()

    var formData = new FormData()
    formData.append('TrainingName', trainingName)
    formData.append('Description', description)
    formData.append('ApplicationDeadline', applicationDeadline)
    formData.append('Capacity', capacity)
    formData.append('DepartmentId', departmentId)
    formData.append('TrainingStartDateTime', trainingStartDateTime)
    formData.append('PreRequisiteIds', preRequisiteIds)

    fetch('/Training/AddTraining', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success(`${data.message}`, "Success", {
                    progressBar: true,
                    timeOut: 1000
                })
                setTimeout(() => {
                    window.location.reload()
                }, 1100)
            }
            else {
                toastr.error(`${data.message}`, "Error", {
                    progressBar: true,
                    timeOut: 2500
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
        .finally(() => {
            var addTrainingCollapse = new bootstrap.Collapse(document.getElementById('addTrainingCollapse'));
            addTrainingCollapse.hide();
            enableButton(document.getElementById('nextBtn'))
        })
}

var modalTrainingId = document.getElementById('modalTrainingId')
function submitEditTrainingForm(editSlimSelect) {
    const trainingId = modalTrainingId.value
    const trainingName = document.getElementById('trainingName').value
    const description = document.getElementById('trainingDescription').value
    const applicationDeadline = document.getElementById('applicationDeadline').value
    const capacity = document.getElementById('capacity').value

    const trainingStartDate = document.getElementById('trainingStartDate').value;
    const trainingStartTime = document.getElementById('trainingStartTime').value;
    const trainingStartDateTime = `${trainingStartDate} ${trainingStartTime}`

    const departmentId = document.getElementById('departments').value
    const preRequisiteIds = editSlimSelect.getSelected()

    var formData = new FormData()
    formData.append('TrainingId', trainingId)
    formData.append('TrainingName', trainingName)
    formData.append('Description', description)
    formData.append('ApplicationDeadline', applicationDeadline)
    formData.append('Capacity', capacity)
    formData.append('DepartmentId', departmentId)
    formData.append('TrainingStartDateTime', trainingStartDateTime)
    formData.append('PreRequisiteIds', preRequisiteIds)

    fetch('/Training/UpdateTraining', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success(`${data.message}`, "Success", {
                    progressBar: true,
                    timeOut: 1000
                })
                setTimeout(() => {
                    window.location.reload()
                }, 1100)
            }
            else {
                toastr.error(`${data.message}`, "Error", {
                    progressBar: true,
                    timeOut: 2500
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
        .finally(() => {
            document.getElementById('saveChangesBtn').disabled = false
            $('#updateTrainingModal').modal('hide');
        })
}

function fetchTrainingDetails(slimSelect, trainingId, button) {
    fetch(`/Training/GetTrainingById?trainingId=${trainingId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const departmentPromise = fetchDepartments();
                const preRequisitesPromise = fetchPreRequisites()

                return Promise.all([departmentPromise, preRequisitesPromise])
                    .then(([departmentsData, preRequisitesData]) => {
                        populateEditTrainingForm(slimSelect, data.training, departmentsData,
                            data.training.DepartmentName, preRequisitesData, data.training.PreRequisites)
                        $('#updateTrainingModal').modal('show');
                    })
            }
            else {
                toastr.error(`${data.message}`, "Error", {
                    progressBar: true,
                    timeOut: 3000
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
        .finally(() => {
            enableButton(button)
        })
}

function populateEditTrainingForm(slimSelect, training, departmentsData,
    currentTrainingDepartment, preRequisitesData, listOfCurrentTrainingPreRequisites) {

    document.getElementById('trainingTitle').textContent = training.TrainingName;
    document.getElementById('trainingName').value = training.TrainingName;
    document.getElementById('trainingDescription').value = training.Description.replace(/\s+/g, ' ').trim();

    const applicationDeadline = document.getElementById('applicationDeadline')
    applicationDeadline.value = formatDateTime(training.DeadlineOfApplication);

    document.getElementById('capacity').value = training.Capacity;
    const departmentSelectElement = document.getElementById('departments')
    populateDepartmentOptions(departmentSelectElement, departmentsData, currentTrainingDepartment, true)

    var trainingStartDate = document.getElementById('trainingStartDate');
    trainingStartDate.value = formatDateTime(training.TrainingCourseStartingDateTime);
    const currentDate = new Date();
    const formattedCurrentDate = currentDate.toISOString().split('T')[0];
    // Limit deadline, initial call
    updateDeadlineMinMaxLimits(applicationDeadline, formattedCurrentDate, trainingStartDate.value);
    trainingStartDate.min = formattedCurrentDate
    // On change, limit deadline
    trainingStartDate.addEventListener('change', function () {
        updateDeadlineMinMaxLimits(applicationDeadline, formattedCurrentDate, trainingStartDate.value)
    });
    document.getElementById('trainingStartTime').value = formatDateTime(training.TrainingCourseStartingDateTime, true);
    populatePreRequisitesOptions(slimSelect, preRequisitesData, listOfCurrentTrainingPreRequisites, true)
}

function updateDeadlineMinMaxLimits(applicationDeadlineInput, formattedCurrentDate, trainingStartDate) {
    // Update the min and max attributes of the deadline input
    applicationDeadlineInput.min = formattedCurrentDate;
    applicationDeadlineInput.max = trainingStartDate;
}

function formatDateTime(timestamp, isTime = false) {
    // Extracting only unix timestamp
    var match = timestamp.match(/\d+/);
    var numericPart = match ? match[0] : null;

    // Make sure to treat as millisecond
    const dateObject = new Date(parseInt(numericPart, 10))

    if (isTime) {
        const hours = String(dateObject.getHours()).padStart(2, '0');
        const minutes = String(dateObject.getMinutes()).padStart(2, '0');
        return `${hours}:${minutes}`
    }
    else {
        const day = String(dateObject.getDate()).padStart(2, '0')
        const month = String(dateObject.getMonth() + 1).padStart(2, '0')
        const year = dateObject.getFullYear()
        return `${year}-${month}-${day}`
    }
}

function fetchPreRequisites() {
    return new Promise((resolve, reject) => {
        fetch('/Training/GetAllPreRequisites', {
            method: 'GET'
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    resolve(data.preRequisites)
                }
                else {
                    toastr.error("An error occurred while fetching all pre-requisites", "Error", {
                        progressBar: true,
                        timeOut: 3000
                    });
                    reject(new Error("Failed to fetch all pre-requisites"));
                }
            })
            .catch(error => {
                window.location.href = '/Common/InternalServerError';
                reject(error);
            })
    })
}

function populatePreRequisitesOptions(slimSelect, listOfAllPreRequisites, listOfCurrentTrainingPreRequisites, isUpdate) {
    const formattedArrayOfAllPreRequisites = listOfAllPreRequisites.map(preRequisite => ({
        text: `${preRequisite.Name}: ${preRequisite.PreRequisiteDescription}`,
        value: `${preRequisite.Id}`
    }));
    slimSelect.setData(formattedArrayOfAllPreRequisites);

    if (isUpdate) {
        const valuesToSelect = listOfCurrentTrainingPreRequisites.map(preRequisite => preRequisite.PreRequisiteId.toString());
        slimSelect.setSelected(valuesToSelect);
    }
}

function fetchDepartments() {
    return new Promise((resolve, reject) => {
        fetch('/Account/GetAllDepartments', {
            method: 'GET'
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    resolve(data.departments)
                }
                else {
                    toastr.error("An error occurred while fetching departments", "Error", {
                        progressBar: true,
                        timeOut: 3000
                    });
                    reject(new Error("Failed to fetch departments"));
                }
            })
            .catch(error => {
                window.location.href = '/Common/InternalServerError';
                reject(error);
            })
    })
}

function populateDepartmentOptions(departmentSelectElement, departments, currentTrainingDepartment, isUpdate) {
    departmentSelectElement.innerHTML = ''
    departments.forEach(function (department) {
        var option = document.createElement('option')
        option.value = department.Id
        option.text = department.DepartmentName

        if (isUpdate && (department.DepartmentName === currentTrainingDepartment)) {
            option.selected = true
        }
        departmentSelectElement.appendChild(option)
    })
}

function deleteTraining(button, trainingId) {
    fetch(`/Training/Delete?trainingId=${trainingId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                toastr.success(`${data.message}`, "Success", {
                    progressBar: true,
                    timeOut: 1250
                })
                setTimeout(() => {
                    window.location.reload()
                }, 1500)
            }
            else {
                toastr.error(`${data.message}`, "Cannot delete training", {
                    progressBar: true,
                    timeOut: 2500
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError';
        })
        .finally(() => {
            setTimeout(() => {
                enableButton(button)
            }, 1450)
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
            var trainingId = button.getAttribute('data-training-id');
            modalTrainingId.value = trainingId
            action(button, trainingId);
        };
        button.addEventListener('click', button.clickHandler);
    });
}

function clearForm(form) {
    Array.from(form.elements).forEach(element => {
        if (['input', 'textarea', 'select'].includes(element.tagName.toLowerCase())) {
            element.value = '';
        }
    });
}
