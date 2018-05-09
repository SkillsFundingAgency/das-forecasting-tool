(function () {
    var init = function () {
        if ($("#choose-apprenticeship")) {
            $("#choose-apprenticeship").select2();
        }
    };
    init();

    // open dropdownon on focus
    $(document).on('focus', '.select2', function () {
        $(this).siblings('select').select2('open');
    });

    // retain tabbed order after selection
    $('#choose-apprenticeship').on('select2:select', function () {
        $("#no-of-app").focus();
    });

    // retain tabbed order on close without selection
    $('#choose-apprenticeship').on('select2:close', function () {
        $("#no-of-app").focus();
    });

}());