$(function () {

    $('.number-only').bind('keyup paste', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    $("#frmContactUs").submit(function (e) {
        e.preventDefault();

        var frmData = {
            FullName: $('#txtFullName').val(),
            StudentPhone: $('#txtStudentPhone').val(),
            ParentPhone: $('#txtParentPhone').val(),
            CourseName: $("#selectCourseName").val(),
            Comment: $('#txtComment').val()
        };

        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/api/contact',
            type: 'post',
            contentType: 'application/json', 
            dataType: 'json', 
            data: JSON.stringify(frmData), 
            headers: {
                'RequestVerificationToken': token  
            },

            success: function () {                
                $('#contactHolder').html("");
                $('#contactHolder').html("<p class='lead'>Thank you for taking the time to review my inquiry. We will contact you within 24 hours. If you need any further details or clarification, feel free to reach out our customer service.</p>");
            },

            error: function (error) {
                $('#contactHolder').html("");
                $('#contactHolder').html("<p class='lead text-danger'>Oops! Something went wrong while submitting your form. Please try again later.</p>");                
            }
        });
    });
});