﻿
var AddEditApprentiecships = {

    altFind: function (arr, callback) {
        for (var i = 0; i < arr.length; i++) {
            var match = callback(arr[i]);
            if (match) {
                return arr[i];
            }
        }
    },

    calculateFundingCap: function (date, model) {
        var today = new Date();
        var thisMonth = new Date(today.getFullYear(), today.getMonth(), 1, 0, 0, 0)

        if (date === undefined
            || date.toString() === "Invalid Date"
            || date < thisMonth
            || model === undefined
            || model.FundingBands == null ){
            return undefined;
        }

        var fundingBand = AddEditApprentiecships.altFind(model.FundingBands, function (fb) {
            return date > AddEditApprentiecships.getDate(fb.FromDate) && date < AddEditApprentiecships.getDate(fb.ToDate);
        }) || model.FundingBands[model.FundingBands.length - 1];

        var result = {
            FundingCap: fundingBand.FundingCap
        };

        return result;
    },

    getDate: function (cSharpDate) {
        if (!cSharpDate)
            return cSharpDate;

        if (cSharpDate.indexOf('-') > -1) {
            return new Date(cSharpDate);
        }

        var stripedCsharpDate = cSharpDate.replace(/[^0-9 +]/g, '');
        return new Date(parseInt(stripedCsharpDate));
    },

    toGBP: function (data) {
        return data.toLocaleString('en-GB', { style: 'currency', currency: 'GBP' }).split('.')[0];
    },

    numberWithCommas: function (number) {
        var parts = number.toString().split('.');
        var partToProcess = parts[0];
        partToProcess = partToProcess.replace(/,/g, '');
        partToProcess = partToProcess.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        parts[0] = partToProcess;
        return parts.join('.');
    }, 

    onlyAllowNumbers: function (event) {
        return event.metaKey ||
            event.which <= 0 ||
            event.which == 8 ||
            /[0-9]/.test(String.fromCharCode(event.which));
    }
    
};

// For Jasmine testing
if (typeof (module) != 'undefined') {
    module.exports = AddEditApprentiecships;
}