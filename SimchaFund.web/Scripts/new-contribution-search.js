$(function () {
  

    $("#search").on('keyup', function () {
        var text = $(this).val();
        $("table tr:gt(0)").each(function () {
            var tr = $(this);
            var name = tr.find('td:eq(2)').text();
            if (name.toLowerCase().indexOf(text.toLowerCase()) !== -1) {
                tr.show();
            } else {
                tr.hide();
            }
        });
    });

   
    $("#clear").on('click', function () {
        $("#search").val('');
        $("tr").show();
    });

})