$(document).ready(function () {

    $('#selectBatch').on('change', function () {
        var id = $(this).val();
        if (!id) {
            $('#inputFees').val("");
            $('#inputDiscount').val("");
            $('#spanTotal').text("");
            return;
        };

        var batchData = $('#batchScript').text();
        var batchList = JSON.parse(batchData);
        var batchModel = batchList.find(x => x.Id == id);
        $('#inputFees').val(batchModel.Fee.Amount);
        $('#spanTotal').text(batchModel.Fee.Amount);



    });

    $('.installment-date').datetimepicker({
        autoclose: true,
        timepicker: false,
        mask: false,
    });

    $('#inputDiscount').keyup(function () {

        var amt = $('#inputFees').val();
        $('#spanTotal').text(amt);
        var discount = parseInt($(this).val());

        if (discount) {
            $('#spanTotal').text(amt - discount);
            $('#txtFirstInstallment').val(amt - discount);
        }
    });

    $('#txtFirstInstallment').keyup(function () {
        
        var amt = parseInt($('#inputFees').val());
        var discount = parseInt($('#inputDiscount').val());
        var totalFees = (amt - discount);

        if ($('#txtFirstInstallment').val() != "") {
            var firstInstallment = parseInt($('#txtFirstInstallment').val());
            var remaining = totalFees - firstInstallment;
            $('#txtSecondInstallment').val(remaining);
        }

    });

    $('#txtSecondInstallment').keyup(function () {

        var amt = parseInt($('#inputFees').val());
        var discount = parseInt($('#inputDiscount').val());
        var totalFees = (amt - discount);

        if ($('#txtFirstInstallment').val() != "" && $('#txtSecondInstallment').val() != "") {
            var firstInstallment = parseInt($('#txtFirstInstallment').val());
            var secondInstallment = parseInt($('#txtSecondInstallment').val());
            var remaining = firstInstallment + secondInstallment;
            var thidInstallment = totalFees - remaining;
            $('#txtThirdInstallment').val(thidInstallment);
        }

    });


});