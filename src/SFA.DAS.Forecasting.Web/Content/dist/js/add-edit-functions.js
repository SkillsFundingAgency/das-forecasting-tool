
var AddEditApprentiecships = {
    calculateFundingCap: function (date, model) {
        var today = new Date();
        var thisMonth = new Date(today.getFullYear(), today.getMonth(), 1, 0, 0, 0)

        if (date === undefined
            || date.toString() === "Invalid Date"
            || date < thisMonth
            || model === undefined) {
            return undefined;
        }

        var fundingBand = model.FundingBands.find((fb) => {
            return date > this.getDate(fb.FromDate) && date < this.getDate(fb.ToDate)
        })
            || model.FundingBands[model.FundingBands.length - 1];

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
    }
};

// For Jasmine testing
if (typeof (module) != 'undefined') {
    module.exports = AddEditApprentiecships;
}