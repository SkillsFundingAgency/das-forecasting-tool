var sfa = sfa || {};
sfa.AddApprenticeship = sfa.AddApprenticeship || {};

(function () {

    var init = function () {
        var fundingBands = JSON.parse($('#FundingPeriodsJson').val())
        sfa.AddApprenticeship.Result = {
            CourseId: $('#CourseId').val(),
            NumberOfMonths: $('#levy-length').val(),
            FundingBands: fundingBands,
        };

        $("#no-of-app").change(function () {
            if (sfa.AddApprenticeship.Result)
                calculateTotalCostLocal();
        });

        $("#startDateMonth, #startDateYear").change(function () {
            if (sfa.AddApprenticeship.Result)
                calculateTotalCostLocal();  
        });

        $("#total-funding-cost").keyup(function () {
            var num = $('#total-funding-cost').val();
            var commaNum = AddEditApprentiecships.numberWithCommas(num);
            $('#total-funding-cost').val(commaNum);
        });
    }

    if (document.getElementById("estimate-edit-apprenticeship") != undefined) {
        init();
    }

    function resetNumberOfMonths() {
        var courseId = $('#choose-apprenticeship').val();
        var previousCourseId = $('#PreviousCourseId').val();
        if (courseId !== previousCourseId) {
            var requestObject = {
                courseId: courseId
            };
        }
    }

    function calculateTotalCostLocal() {
        var model = GetAddApprenticeshipForm();

        var updated = refreshCalculation(model, sfa.AddApprenticeship.Result);
        showFundingCapMessage(updated);

    }

    function refreshCalculation(model, result) {
        var calc = AddEditApprentiecships.calculateFundingCap(model.StartDate, result)

        var fc = calc ? calc.FundingCap : 0;
        model.FundingCap = fc;
        model.TotalFundingCap = fc * model.NumberOfApprentices;

        updateView(model);

        return model.FundingCap > 0 && model.TotalFundingCap > 0;
    }

    function GetAddApprenticeshipForm() {
        return {
            CourseId: $('#choose-apprenticeship').val(),
            NumberOfApprentices: parseInt($('#no-of-app').val()),
            LevyValue: parseFloat($('#total-funding-cost').val()),
            StartDate: new Date($('#startDateYear').val(), $('#startDateMonth').val() - 1, 1),
            NumberOfMonths: parseInt($('#apprenticeship-length').val() || 0)
        }
    }

    function updateView(result) {
        var fc = AddEditApprentiecships.toGBP(result.FundingCap)
        var tfc = AddEditApprentiecships.toGBP(result.TotalFundingCap || 0)

        $('#funding-cap-details').html(fc);
        $('#apprentice-count-details').html(result.NumberOfApprentices);
        $('#total-cap-details').html(tfc);
        $('#total-funding-cost').val(tfc.replace('£', ''));
    }

    function showFundingCapMessage(show) {
        if (show) {
            $('#details-about-funding').hide();
            $('#details-about-funding-calculated').show();
        } else {
            $('#details-about-funding').show();
            $('#details-about-funding-calculated').hide();
        }
    }

}());