$(document).ready(function () {
    $('#selectBatch').select2({ theme: "bootstrap-5" });
    $('#selectSubject').select2({ theme: "bootstrap-5" });

    $('#inputStartDate').datetimepicker({
        autoclose: true

    });
    $('#inputEndDate').datetimepicker({
        autoclose: true
    });

    $('#selectExamMode').on('change', function () {

        var modeId = $(this).find(":selected").val();
        if (modeId == 1) {
            $('#spanFileType').text('MS Word');
        }
        else {
            $('#spanFileType').text('MS Word/PDF');
        }

    });

    $("#questionFile").on("change", function (event) {
        $('#fileMessage').text("");
        var file = event.target.files[0];
        if (!file) return;

        const allowedExtensions = ["application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"];
        if (!allowedExtensions.includes(file.type)) {
            $('#fileMessage').text("Invalid file type! Please select a Word document (.doc, .docx) or PDF.");
            this.value = "";
        }
        if ($('#selectExamMode').val() == "1" && file.type != "application/vnd.openxmlformats-officedocument.wordprocessingml.document") {
            $('#fileMessage').text("Question paper for Online exam mode must be a MS Word template");
            this.value = "";

        }
    });

});