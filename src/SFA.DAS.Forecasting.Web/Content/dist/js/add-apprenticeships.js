var AddApprentiecships = {

    calculateFundingCap: function (date, model) {
        var today = new Date();
        var thisMonth = new Date(today.getFullYear(), today.getMonth(), 1, 0, 0, 0)

        if (   date === undefined
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

        var stripedCsharpDate = cSharpDate.replace(/[^0-9 +]/g, '');
        return new Date(parseInt(stripedCsharpDate));
    }
}

// For Jasmine testing
if (typeof(module) != 'undefined') {
    module.exports = AddApprentiecships;
}