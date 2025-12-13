function loadStockModel(id) {
    $("#txtQuantity").val("");
    $("#labelProductName").text($(`#title-${id}`).text());
    $("#txtProductId").val(id);

    var stockModel = new bootstrap.Modal($("#stockModel"));
    stockModel.show();
}
function addStock() {
    var id = $('#txtProductId').val();
    var frmData = {
        productId: id,
        quantity: $('#txtQuantity').val(),
    };
    $('#btnAddStock').attr('disabled', 'disabled');
    $.ajax({
        type: 'post',
        url: '/api/stock',
        data: frmData,
        success: function (response) {
            $('#btnAddStock').removeAttr('disabled');
            $('#message').text("Stock added sucessfully.");
        },
        error: function (error) {
            $('#btnAddStock').removeAttr('disabled');
            $('#message').text("There is an error while adding stock. Please referesh this page and try again.");
        }
    });
}
