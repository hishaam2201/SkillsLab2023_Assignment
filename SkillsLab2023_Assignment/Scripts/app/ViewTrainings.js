(function () {
    fetch('/Training/GetAllTrainings', {
        method: 'POST'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                populateTable(data.trainings)
            }
            else {
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 500)
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
        trainingProgrammeCell.textContent = training.ProgrammeName
        row.appendChild(trainingProgrammeCell)

        const departmentCell = document.createElement('td')
        departmentCell.textContent = training.DepartmentName
        row.appendChild(departmentCell)

        const deadlineCell = document.createElement('td')
        deadlineCell.textContent = formatTimestamp(training.Deadline)
        row.appendChild(deadlineCell)

        const capacityCell = document.createElement('td')
        capacityCell.textContent = training.Capacity
        row.appendChild(capacityCell)

        const viewButtonCell = document.createElement('td')
        const viewButton = document.createElement('button')
        viewButton.classList.add('btn');
        viewButton.classList.add('btn-primary');
        viewButton.textContent = 'View more'
        viewButton.addEventListener('click', () => {
            window.location.href = `/Training/Details/${training.TrainingId}`;
        })
        viewButtonCell.appendChild(viewButton)
        row.appendChild(viewButtonCell)

        tableBody.appendChild(row)
    })
}

function formatTimestamp(timestamp) {
    const unixTimeMilliseconds = parseInt(timestamp.match(/\d+/)[0]);

    const date = new Date(unixTimeMilliseconds)

    const day = date.getDate().toString().padStart(2, '0')
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const year = date.getFullYear()

    const formattedDate = `${day}/${month}/${year}`

    return formattedDate
}