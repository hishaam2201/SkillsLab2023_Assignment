(function () {
    var modal = document.getElementById('declineMessageModal')
    var textArea = modal.querySelector('#message-text')

    var userApplications = document.getElementById('userApplications');
    new DataTable(userApplications, {
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
            addButtonClickListener('decline-btn', function (button) {
                disableButton(button)
                var dataDeclineReason = button.getAttribute('data-decline-reason');
                dataDeclineReason == '' ? textArea.value = 'Not applicable' : textArea.value = dataDeclineReason.replace(/\s+/g, ' ').trim();
                setTimeout(() => {
                    enableButton(button)
                }, 500)
            }, textArea)
        }
    });
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

function addButtonClickListener(className, action, textArea) {
    var buttons = document.querySelectorAll(`.${className}`);
    buttons.forEach(function (button) {
        button.removeEventListener('click', button.clickHandler);
        button.clickHandler = function () {
            action(button, textArea);
        };
        button.addEventListener('click', button.clickHandler);
    });
}