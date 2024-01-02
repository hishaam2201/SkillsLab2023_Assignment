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
            addButtonClickListener('edit-btn', function (_, trainingId) {
                fetchTrainingDetails(slimSelect, trainingId)
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

function fetchTrainingDetails(slimSelect, trainingId) {
    fetch(`/Training/GetTrainingById?trainingId=${trainingId}`, {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                fetchDepartments(data.training.DepartmentName)
                fetchPreRequisites(slimSelect, data.training.PreRequisites)
                //TODO: Populate Training form with existing data
                //console.log(data.training)
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
}

function fetchPreRequisites(slimSelect, listOfCurrentTrainingPreRequisites) {
    fetch('/Training/GetAllPreRequisites', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                populatePreRequisitesOptions(slimSelect, data.preRequisites, listOfCurrentTrainingPreRequisites)
            }
            else {
                toastr.error("Pre Requisites could not be fetched", "Error", {
                    progressBar: true,
                    timeOut: 3000
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
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

function fetchDepartments(currentTrainingDepartment) {
    fetch('/Account/GetAllDepartments', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                populateDepartmentOptions(data.departments, currentTrainingDepartment)
            }
            else {
                toastr.error("An error occurred while fetching departments", "Error", {
                    progressBar: true,
                    timeOut: 3000
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
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
        toggleElementVisibility(button, '.trashIcon', true)
        button.disabled = false;
    }
}

function disableButton(button) {
    if (button) {
        toggleElementVisibility(button, '.spinner', true)
        toggleElementVisibility(button, '.trashIcon', false)
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