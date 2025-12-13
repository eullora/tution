var subjectObject;
$(document).ready(function () {
    $('#subjectList').select2({ theme: "bootstrap-5" });
    subjectObject = JSON.parse($('#scriptSubjects').text());

    $("#resultFile").on("change", function (event) {
        var file = event.target.files[0];
        if (!file) return;

        Papa.parse(file, {
            complete: function (results) {
                var headers = results.data[0];
                $('#divHeaders').html("");
                var table = '<table class="table"><thead><tr>';
                headers.forEach(function (value, _) {
                    table = table + '<th>' + value + '</th>';
                });

                table = table + "</tr></thead><tbody><tr>";
                headers.forEach(function (value, idx) {
                    table = table + '<td><input type="checkbox" class="form-check-input code-value" code-idx="' + idx + '" code="' + value.trim() + '" onchange="onHeaderSelect()" ></td>';
                });

                table = table + '</tr></tbody></table>';

                $('#divHeaders').html(table);
            }
        });
    });
});

function onHeaderSelect() {
    var items = [];
    var subjectColumn = [];
    jQuery('.code-value').each(function () {
        var elem = $(this);

        if (elem.prop('checked')) {
            var code = $(elem).attr('code').toUpperCase();
            var codeIndex = $(elem).attr('code-idx');

            var subject = subjectObject.find(w => w.Code == code);
            if (subject) {
                items.push(subject.Id);
                subjectColumn.push(subject.Id + '@' + codeIndex);
            }
        }
    });

    $('#subjectList').val(items);
    $('#subjectList').trigger('change');
    $('#subjectColumns').val(subjectColumn.length > 0 ? subjectColumn.join("#") : "");
}
