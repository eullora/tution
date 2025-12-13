var _selectedInstallmentId = 0;

function loadPaymentModel() {
    $("#txtAmount").val("");
    $("#selectPayType").val("");
    var payModel = new bootstrap.Modal($("#paymentModel"));
    payModel.show();
}

function addPaymentTransaction() {
    var frmData = {
        payTermId: _selectedInstallmentId,
        amount: $(`#txtAmount-${_selectedInstallmentId}`).val(),
        studentId: $('#txtStudentId').val(),
        remark: $('#txtRemark').val()
    };
    console.log(frmData);

    $('#btnAddPayment').attr('disabled', 'disabled');
    $.ajax({
        type: 'post',
        url: '/api/paynow',
        data: frmData,
        success: function (response) {
            $('#message').text("Fee payment completed sucessfully.");
            $('#btnAddPayment').html('<i class="fa-solid fa-spinner fa-spin"></i>');
            setTimeout(function () {
                $('#inputRollNumber').val($('#txtRollNumber').val());
                $('#frmSearch').submit();
            }, 3000);
        },
        error: function (error) {
            $('#btnAddPayment').removeAttr('disabled');
            $('#message').text("There is an error while adding payment. Please referesh this page and try again.");
        }
    });
}

function openPrintWindow(htm) {    
    var win = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if (!win) {
        console.error('Unable to open new window. Check browser popup settings.');
        return;
    }
    win.document.open();
    win.document.write(`<html>
               <head>
                   <link rel="stylesheet" href="/css/styles.css">
                   <link rel="stylesheet" href="/css/fees.css">
                   
                   <style>
                   @media print {
                           html, body {
                               height: 99%;
                           }
                       }
                   </style>
               </head>
               <body style="margin: 0;" onload="window.print(); window.close();">                    
                    ${htm}
               </body>
           </html>`);
    win.document.close();
}