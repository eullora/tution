$(document).ready(function () {
    var mdl = new bootstrap.Modal($("#attendanceModel"));

    $(".view-attendance").on("click", function () {
        var id = $(this).attr("itemid");   
        $('#attendanceTable').html('');
        $.ajax({
            type: 'get',
            url: `/api/${id}/presents`,
            success: function (result) {
                console.log(result);
                var html = ['<table class="table"><tr><th>Student Name</th><th>Roll Number</th><th>Check In Time</th><th>Checkout Time</th></tr>'];
                $(result).each(function (_, attn) {
                    html.push(`<tr><td>${attn.student.fullName}</td><td>${attn.student.rollNumber}</td><td>${attn.checkInTime}</td><td>${attn.checkOutTime ? attn.checkOutTime : "" }</td></tr>`);
                });
                html.push('</table>');
                $('#attendanceTable').html(html.join(''));
                mdl.show();
            },
            error: function (error) {
                $('#message').text("There is an error while loading attenance. Please referesh this page and try again.");
                console.log(error);
            }
        });
    });
});