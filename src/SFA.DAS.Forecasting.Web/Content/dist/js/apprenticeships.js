var sfa = sfa || {};

(function () {

    var init = function () {
        if (document.getElementById("estimate-edit-apprenticeship")) {
            $("#no-of-app").keyup(function () {
                calculateTotalCost();
            });
        }
    };

    init();

    function calculateTotalCost () {

        var noOfApprentices = parseInt($('#no-of-app').val()) || 0;

        var course = document.getElementById('course');
        var fundingCap = course.dataset.fundingCap

        var totalCap = (noOfApprentices * fundingCap) || 0;
        $('#apprentice-count-details').text(noOfApprentices);

        var totalCapFormated = toGBP(totalCap)
        $('#total-cap-details').text(totalCapFormated);
        $('#total-funding-cost').val(numberWithCommas(totalCapFormated).replace('£',''));

        if (noOfApprentices > 0) {
            $('#details-about-funding').hide()
            $('#details-about-funding-calculated').show()
        } else {
            $('#details-about-funding').show();
            $('#details-about-funding-calculated').hide();
        }
    }

    function toGBP(data) {
        return data.toLocaleString('en-GB', { style: 'currency', currency: 'GBP' }).split('.')[0];
    }

    function numberWithCommas(number) {
        var parts = number.toString().split('.');
        var partToProcess = parts[0];
        partToProcess = partToProcess.replace(/,/g, '');
        partToProcess = partToProcess.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        parts[0] = partToProcess;
        return parts.join('.');
    }

}());