(function () {
    fetch('/Training/GetAllTrainings', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                if (data.trainings && data.trainings.length > 0) {
                    populateTable(data.trainings)
                }
                else {
                    toastr.warning("No trainings found", {
                        timeOut: 5000,
                        progressBar: true
                    })
                }
            }
            else {
                toastr.error("Trainings could not be fetched", "Error", {
                    timeOut: 5000,
                    progressBar: true
                })
            }
        })
        .catch(() => {
            window.location.href = '/Common/InternalServerError'
        })
})()

function populateTable(trainings) {
    const tableBody = document.getElementById('tableBody')

    trainings.forEach(training => {
        const row = document.createElement('tr')

        const trainingProgrammeCell = document.createElement('td')
        trainingProgrammeCell.textContent = training.TrainingName
        row.appendChild(trainingProgrammeCell)

        const departmentCell = document.createElement('td')
        departmentCell.textContent = training.DepartmentName
        row.appendChild(departmentCell)

        const deadlineCell = document.createElement('td')
        deadlineCell.textContent = training.DeadlineOfApplication
        row.appendChild(deadlineCell)

        const viewButtonCell = document.createElement('td')
        const viewButton = document.createElement('button')
        viewButton.classList.add('btn');
        viewButton.classList.add('btn-primary');
        viewButton.textContent = 'View more';
        viewButton.addEventListener('click', () => {
            document.getElementById('trainingIdInput').value = training.TrainingId;
            document.getElementById('trainingForm').submit();
        })
        viewButtonCell.appendChild(viewButton)
        row.appendChild(viewButtonCell)

        tableBody.appendChild(row)
    })
}
