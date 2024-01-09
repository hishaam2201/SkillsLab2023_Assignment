(function () {
    var allTrainingsTable = document.getElementById('allTrainings');
    new DataTable(allTrainingsTable, {
        responsive: true,
        paging: true,
        // order: [[0, 'desc']],
        /*columnDefs: [
            {
                "targets": [2, 5, 6, 7, 8],
                "orderable": false,
            }
        ],*/
    });
})()