var chart = (function () { 

    var dataContainer = document.getElementsByClassName('chart-data')[0];

    var dates = dataContainer.dataset.dates.split(',');
    var values = dataContainer.dataset.values.split(',');

    var chart = c3.generate({
        bindto: '#chart',
        legend: { show: false },
        point: { show: false },
        interaction: { enabled: false },
        data: {
            x: 'x',
            columns: [
                ['x'].concat(dates),
                ['data1'].concat(values)
            ],
            type: 'spline',
            axes: { data1: 'y2' },
        },
        axis: {
            y2: {
                show: true
            },
            y: { show: false },
            x: {
                type: 'timeseries',
                tick: {
                    fit: true,
                    count: 6,
                    format: "%b %y"
                }
            }
        },
        padding: {
            bottom: 40,
            left: 10,
            right: 40
        },
    });

    document.getElementsByClassName('chart-container')[0].style.display = "block";

});

window.onload = function () {

    if (document.getElementsByClassName('chart-container').length !== 0)
    {
        chart();
    }
};

//OR use Object http://jsfiddle.net/etuwo8mz/57/
//http://stackabuse.com/how-to-format-dates-in-javascript/