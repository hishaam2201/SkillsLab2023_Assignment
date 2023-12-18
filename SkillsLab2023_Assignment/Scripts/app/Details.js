
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
        const maxSizeInBytes = 1024 * 1024 * 10; 
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
        // submit form
        form.classList.remove('was-validated')
    }

    form.classList.add('was-validated')
})

function displayToastToUser(toastColor, message) {
    if (toastColor === 'success') {
        toastr.success(`${message}`, "Success", {
            timeOut: 1500,
            progressBar: true
        })
    }
    else {
        toastr.error(`${message}`, "Error", {
            timeOut: 5000,
            progressBar: true
        })
    }
}