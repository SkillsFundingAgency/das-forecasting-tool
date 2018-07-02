'use strict';

var addEditApprenticeship = require('../dist/js/add-edit-functions');
var today = new Date();
var model =
    {
        CourseId: 12,
        FundingBands: [
            { FromDate: "/Date(" + startOfYear(today.getFullYear()).getTime() + ")/", ToDate: "/Date(" + endOfYear(today.getFullYear()).getTime() + ")/", FundingCap: 100 },
            { FromDate: "/Date(" + startOfYear(today.getFullYear() + 1).getTime() + ")/", ToDate: "/Date(" + endOfYear(today.getFullYear() + 1).getTime() + ")/", FundingCap: 200 },
            { FromDate: "/Date(" + startOfYear(today.getFullYear() + 2).getTime() + ")/", ToDate: 'null', FundingCap: 300 }
        ]
    }

function startOfYear(year){
    return new Date(year, 0, 1);
}

function endOfYear(year) {
    return new Date(year, 11, 31);
}

describe('add apprenticeships', () => {

    it('calculate funding cap', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear());

        var result = addEditApprenticeship.calculateFundingCap(date, model);
        expect(result.FundingCap).toBe(100);
    });

    it('calculate funding cap when date is middle year', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 1);

        var result = addEditApprenticeship.calculateFundingCap(date, model);
        expect(result.FundingCap).toBe(200);
    });

    it('calculate funding cap when date is last year', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 2);

        var result = addEditApprenticeship.calculateFundingCap(new Date(date.getFullYear(), 2, 1), model);
        expect(result.FundingCap).toBe(300);
    });

    it('calculate funding cap when date is after last funding band', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 4);

        var result = addEditApprenticeship.calculateFundingCap(new Date(date.getFullYear(), 1, 1), model);
        expect(result.FundingCap).toBe(300);
    });

    it('is undefined if date is not valid', () => {
        var date = new Date(-1, "abba", 1);
        var result = addEditApprenticeship.calculateFundingCap(date, model);
        expect(result).toBeUndefined()
    });

    it('is undefined if date is undefined', () => {
        var result = addEditApprenticeship.calculateFundingCap(undefined, model);
        expect(result).toBeUndefined()
    });

    it('is undefined if date is in the past', () => {
        var date = new Date();
        date.setMonth(date.getMonth() - 1);
        var result = addEditApprenticeship.calculateFundingCap(date, model);
        expect(result).toBeUndefined()
    });

});
describe('when parsing date', () => {

    it('returns undefinied if input is undefined', () => {
        var result = addEditApprenticeship.getDate(undefined);
        expect(result).toBeUndefined();
    });

    it('should parse datetime', () => {
        var result = addEditApprenticeship.getDate('1998-12-08');
        expect(result).toBeCloseTo(new Date(1998, 11, 8));
    });

    it('should parse datetime in milliseconds', () => {
        var result = addEditApprenticeship.getDate('913075200000');
        expect(result).toBeCloseTo(new Date(1998, 11, 8));
    });
});

describe('format text', () => {
    it ('1000 should be formated to 1,000', () => {
        var result = addEditApprenticeship.numberWithCommas(1000);
        expect(result).toBe('1,000');
    });

    it('12000 should be formated to 12,000', () => {
        var result = addEditApprenticeship.numberWithCommas(12000);
        expect(result).toBe('12,000');
    });

    it('should format number to currency string', () => {
        var result = addEditApprenticeship.toGBP(13500);
        expect(result).toBe('£13,500');
    });
});