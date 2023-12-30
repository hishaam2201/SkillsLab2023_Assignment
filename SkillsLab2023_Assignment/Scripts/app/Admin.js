document.addEventListener('DOMContentLoaded', function () {
    var allTrainingsTable = document.getElementById('allTrainings');
    var dataTable = new DataTable(allTrainingsTable, {
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
                alert(`Edit button clicked for ${trainingId}`);
            });

            addButtonClickListener('delete-btn', function (button, trainingId) {
                disableButton(button)
                setTimeout(() => {
                    deleteTraining(button, trainingId)
                }, 1500)
            });
        }
    });
});

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