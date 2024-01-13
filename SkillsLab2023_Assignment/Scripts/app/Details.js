
const form = document.getElementById('uploadForm')
form.addEventListener('submit', event => {

    const fileInputs = form.querySelectorAll('input[type="file"]');
    let isValid = true;

    fileInputs.forEach(fileInput => {
        const file = fileInput.files[0];
        const index = fileInput.dataset.index;
        const requiredFeedback = document.getElementById(`requiredFeedback_${index}`);
        const typeFeedback = document.getElementById(`typeFeedback_${index}`);
        const sizeFeedback = document.getElementById(`sizeFeedback_${index}`);

        requiredFeedback.style.display = 'none';
        typeFeedback.style.display = 'none';
        sizeFeedback.style.display = 'none';

        if (!file) {
            isValid = false;
            requiredFeedback.style.display = 'block';
            return;
        }

        // Check file type
        const allowedTypes = ['application/pdf', 'image/jpeg', 'image/png'];
        if (!allowedTypes.includes(file.type)) {
            isValid = false;
            typeFeedback.style.display = 'block';
            return
        }

        // Check file size (in bytes)
        const maxSizeInBytes = 1024 * 1024 * 6; 
        if (file.size > maxSizeInBytes) {
            isValid = false;
            sizeFeedback.style.display = 'block';
            return
        }
    });

    if (!isValid) {
        event.preventDefault()
        event.stopPropagation()
    }
    else {
        submitApplication(form);
        form.classList.remove('was-validated')
    }

    form.classList.add('was-validated')
})

function submitApplication() {
    var formData = new FormData();
    var trainingId = document.getElementById('trainingId').value
    var trainingName = document.getElementById('trainingName').value
    formData.append(`TrainingId`, trainingId)
    formData.append(`TrainingName`, trainingName)

    var fileInputs = document.querySelectorAll('.file-upload');
    var hasPreRequisites = fileInputs.length > 0
    if (hasPreRequisites) {
        fileInputs.forEach(function (fileInput) {
            var index = fileInput.dataset.index;
            var prerequisiteId = document.getElementById("prerequisiteId_" + index).value
            var file = fileInput.files[0]
            var encodedFileName = encodeURIComponent(file.name)

            formData.append(`Files[${index}].PreRequisiteId`, prerequisiteId)
            formData.append(`Files[${index}].File`, file)
            formData.append(`Files[${index}].FileName`, encodedFileName)
        })
    }

    fetch("/Application/Enroll", {
        method: "POST",
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                displayToastToUser("success", `${data.message}`)
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 2500)
            }
            else {
                displayToastToUser("Error", `${data.message}`)
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError';
        })
}

function displayToastToUser(toastColor, message) {
    if (toastColor === 'success') {
        toastr.success(`${message}`, "Success", {
            timeOut: 2000,
            progressBar: true
        })
    }
    else {
        toastr.error(`${message}`, "Error", {
            closeButton: true
        })
    }
}