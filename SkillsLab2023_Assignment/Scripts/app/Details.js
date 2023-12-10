(function () {
    const trainingId = window.location.pathname.split('/').pop();

    fetch(`/Training/GetTrainingById/${trainingId}`, {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                console.log(data.training)
            }
            else {
                setTimeout(() => {
                    window.location.href = data.redirectUrl
                }, 500)
            }
        })
        .catch(error => {
            console.error('Error: ', error)
        })
})();