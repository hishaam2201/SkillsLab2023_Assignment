(function () {
    var slimSelect = new SlimSelect({
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
                "targets": [2, 5, 6, 7],
                "orderable": false,
            }
        ],
        drawCallback: function () {
            addButtonClickListener('edit-btn', function (button, trainingId) {
                disableButton(button)
                setTimeout(() => {
                    fetchTrainingDetails(slimSelect, trainingId, button)
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
})()

const editTrainingForm = document.getElementById('editTrainingForm')

editTrainingForm.addEventListener('submit', event => {
    if (!editTrainingForm.checkValidity()) {
        event.preventDefault()
        event.stopPropagation()
    }
    else {
        console.log('Submitted')
    }
    editTrainingForm.classList.add('was-validated')

}, false)

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

function populateEditTrainingForm(slimSelect, training, departmentsData, currentTrainingDepartment, preRequisitesData, listOfCurrentTrainingPreRequisites) {
    document.getElementById('trainingTitle').textContent = training.TrainingName;
    document.getElementById('trainingName').value = training.TrainingName;
    document.getElementById('trainingDescription').value = training.Description;

    const applicationDeadline = document.getElementById('applicationDeadline')
    applicationDeadline.value = formatDateTime(training.DeadlineOfApplication);

    document.getElementById('capacity').value = training.Capacity;
    populateDepartmentOptions(departmentsData, currentTrainingDepartment)

    var trainingStartDate = document.getElementById('trainingStartDate');
    trainingStartDate.value = formatDateTime(training.TrainingCourseStartingDateTime);
    const currentDate = new Date();
    const formattedCurrentDate = currentDate.toISOString().split('T')[0]
    updateDeadlineAttributes(applicationDeadline, formattedCurrentDate, trainingStartDate.value);
    // On change, limit deadline
    trainingStartDate.addEventListener('change', function () {
        updateDeadlineAttributes(applicationDeadline, formattedCurrentDate, trainingStartDate.value)
    });
    document.getElementById('trainingStartTime').value = formatDateTime(training.TrainingCourseStartingDateTime, true);
    populatePreRequisitesOptions(slimSelect, preRequisitesData, listOfCurrentTrainingPreRequisites)
}

function updateDeadlineAttributes(applicationDeadlineInput, formattedCurrentDate, trainingStartDate) {
    // Update the min and max attributes of the deadline input
    applicationDeadlineInput.min = formattedCurrentDate;
    applicationDeadlineInput.max = trainingStartDate; // You can adjust this based on your specific requirements
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

function populatePreRequisitesOptions(slimSelect, listOfAllPreRequisites, listOfCurrentTrainingPreRequisites) {
    const formattedArrayOfAllPreRequisites = listOfAllPreRequisites.map(preRequisite => ({
        text: `${preRequisite.Name}: ${preRequisite.PreRequisiteDescription}`,
        value: `${preRequisite.Id}`
    }));
    slimSelect.setData(formattedArrayOfAllPreRequisites);

    const valuesToSelect = listOfCurrentTrainingPreRequisites.map(preRequisite => preRequisite.PreRequisiteId.toString());
    slimSelect.setSelected(valuesToSelect);
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

function populateDepartmentOptions(departments, currentTrainingDepartment) {
    var departmentSelectElement = document.getElementById('departments')
    departmentSelectElement.innerHTML = ''
    departments.forEach(function (department) {
        var option = document.createElement('option')
        option.value = department.Id
        option.text = department.DepartmentName

        if (department.DepartmentName === currentTrainingDepartment) {
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
            action(button, trainingId);
        };
        button.addEventListener('click', button.clickHandler);
    });
}