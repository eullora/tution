function loadAccessoriesModel(id) {
    var mdl = new bootstrap.Modal($("#accessoriesModel"));
    $.ajax({
        type: 'get',
        url: `/api/${id}/accessories`,        
        success: function (response) {            
            bindDeliverProducts(response);
            mdl.show();
        },
        error: function (error) {            
            $('#message').text("There is an error while loading student accessories. Please referesh this page and try again.");
        }
    });

   
}
function bindDeliverProducts(result) {
    console.log(result);
    $('#labelStudentName').text(result.student.fullName);
    $('#txtStudentId').val(result.student.id);

    $('.check-product').removeAttr('checked');
    console.log(result.transactions);
    $(result.transactions).each(function (_, trn) {
        console.log(trn);
        $(`#chkProduct-${trn.product.id}`).prop('checked', true);
    });
}

function updateDisbribution() {
    $('#message').text("");
    var productIds = [];
    $('.check-product').each(function (index, obj) {
        if (this.checked === true) {
            productIds.push($(this).attr('product-id'));
        }
    });

    if (productIds.length == 0) {
        $('#message').text("Please select product from the list shown above.");
        return;
    }
    
    var frmData = {
        studentId: $('#txtStudentId').val(),
        productIds 
    };

    $('#btnSave').attr('disabled', 'disabled');

    $.ajax({
        type: 'post',
        url: '/api/distribute',
        data: frmData,
        success: function (response) {
            $('#btnSave').removeAttr('disabled');
            $('#message').text("This request updated sucessfully.");
        },
        error: function (error) {
            $('#btnSave').removeAttr('disabled');
            $('#message').text("There is an error while processing this request. Please referesh this page and try again.");
        }
    });
}

