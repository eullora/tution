function loadAddResult() {
    $('#resultFile').val("");
    var mdl = new bootstrap.Modal($("#addResultModel"));
    mdl.show();
}
function addResult() {
    var id = $('#txtExamId').val();
    var frmData = {
        productId: id,
        quantity: $('#txtQuantity').val(),
    };

    var formData = new FormData();
    var fileInput = $("#fileInput")[0].files[0];

    if (!fileInput) {
        return;
    }

    formData.append("file", fileInput);
    formData.append("ExamId", id);

    $.ajax({
        type: 'POST',
        url: '/api/result/add',
        data: frmData,
        success: function (response) {
            $('#message').text("Stock added sucessfully.");
        },
        error: function (error) {
            $('#message').text("There is an error while adding stock. Please referesh this page and try again.");
        }
    });
}
