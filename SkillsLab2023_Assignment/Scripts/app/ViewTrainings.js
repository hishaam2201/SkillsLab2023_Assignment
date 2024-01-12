(function () {
    const unappliedTrainings = document.getElementById('unappliedTrainings')
    new DataTable(unappliedTrainings, {
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
            addButtonClickListener('view-more-btn', function (button, trainingId) {
                disableButton(button)
                document.getElementById('trainingIdInput').value = trainingId;
                document.getElementById('trainingForm').submit();
                enableButton(button)
            })
        }
    })
})()

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