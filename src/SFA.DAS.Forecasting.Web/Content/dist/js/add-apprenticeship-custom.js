(function () {
    var init = function () {
        if ($("#choose-apprenticeship")) {
            $("#choose-apprenticeship").select2();
        }

        if ($("#estimate-add-apprenticeship")) {

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

            $("#choose-apprenticeship").change(function () {
                calculateTotalCost();
                resetNumberOfMonths();
            });

            $("#no-of-app").change(function () {
                calculateTotalCost();
            });

        }
    };
    init();

    $("#total-funding-cost").keyup(function () {
        var num = $('#total-funding-cost').val();
        var commaNum = numberWithCommas(num);
        $('#total-funding-cost').val(commaNum);
    });

    function numberWithCommas(number) {
        var parts = number.toString().split('.');
        var partToProcess = parts[0];
        partToProcess = partToProcess.replace(/,/g, '');
        partToProcess = partToProcess.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        parts[0] = partToProcess;
        return parts.join('.');
    }

    function resetNumberOfMonths() {
        var courseId = $('#choose-apprenticeship').val();
        var previousCourseId = $('#PreviousCourseId').val();
        if (courseId !== previousCourseId) {
            var requestObject = {
                courseId: courseId
            };

            $.ajax({
                type: "POST",
                url: 'GetDefaultNumberOfMonths',
                data: JSON.stringify(requestObject),
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    $('#apprenticeship-length').val(result.NumberOfMonths);
                    $('#PreviousCourseId').val(courseId);
                }
            });
        }
    }

    function calculateTotalCost() {
        var courseId = $('#choose-apprenticeship').val();
        var noOfApprentices = parseInt($('#no-of-app').val());
        var levyValue = parseFloat($('#total-funding-cost').val());

        var requestObject = {
            courseId: courseId,
            numberOfApprentices: noOfApprentices,
            levyValue: levyValue
        };

        if (courseId !== "" && noOfApprentices > 0) {
            $.ajax({
                type: "POST",
                url: 'CalculateTotalCost',
                data: JSON.stringify(requestObject),
                contentType: "application/json; charset=utf-8",
                success: function (result) {

                    $('#funding-cap-details').html(result.FundingCap);
                    $('#apprentice-count-details').html(result.NumberOfApprentices);
                    $('#total-cap-details').html(result.TotalFundingCap);
                    $('#total-funding-cost').val(result.TotalFundingCapValue);

                    $('#details-about-funding').hide();
                    $('#details-about-funding-calculated').show();
                }
            });

        } else {
            $('#details-about-funding').show();
            $('#details-about-funding-calculated').hide();
        }
    }
}());