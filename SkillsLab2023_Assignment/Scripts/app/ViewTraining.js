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

        const startingDateCell = document.createElement('td')
        startingDateCell.textContent = formatTimestamp(training.StartingDate)
        row.appendChild(startingDateCell)

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
            alert(`Viewing details for training with ID: ${training.TrainingId}`)
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

/*Deadline
:
"/Date(1701288000000)/"*/