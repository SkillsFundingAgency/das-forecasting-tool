var sfa = sfa || {};

(function () {

    var init = function () {
        if (document.getElementById("estimate-edit-apprenticeship")) {
            calculateTotalCost();
        }
    };

    init();

    $("#no-of-app").keyup(function () {
        calculateTotalCost();
    });

    function calculateTotalCost () {

        var noOfApprentices = parseInt($('#no-of-app').val()) || 0;

        var course = document.getElementById('course');
        var fundingCap = course.dataset.fundingCap

        var totalCap = (noOfApprentices * fundingCap) || 0;
        $('#apprentice-count-details').text(noOfApprentices);

        $('#total-cap-details').text('£' + totalCap);
        $('#levy-value').val(totalCap);

        if (noOfApprentices > 0) {
            $('#details-about-funding').hide()
            $('#details-about-funding-calculated').show()
        } else {
            $('#details-about-funding').show();
            $('#details-about-funding-calculated').hide();
        }
    } 
}());