'use strict';

var addApprentiecships = require('../dist/js/add-apprenticeships');
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

        var result = addApprentiecships.calculateFundingCap(date, model);
        expect(result.FundingCap).toBe(100);
    });

    it('calculate funding cap when date is middle year', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 1);

        var result = addApprentiecships.calculateFundingCap(date, model);
        expect(result.FundingCap).toBe(200);
    });

    it('calculate funding cap when date is last year', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 2);

        var result = addApprentiecships.calculateFundingCap(new Date(date.getFullYear(), 2, 1), model);
        expect(result.FundingCap).toBe(300);
    });

    it('calculate funding cap when date is after last funding band', () => {
        var date = new Date();
        date.setFullYear(date.getFullYear() + 4);

        var result = addApprentiecships.calculateFundingCap(new Date(date.getFullYear(), 1, 1), model);
        expect(result.FundingCap).toBe(300);
    });

    it('is undefined if date is not valid', () => {
        var date = new Date(-1, "abba", 1);
        var result = addApprentiecships.calculateFundingCap(date, model);
        expect(result).toBeUndefined()
    });

    it('is undefined if date is undefined', () => {
        var result = addApprentiecships.calculateFundingCap(undefined, model);
        expect(result).toBeUndefined()
    });

    it('is undefined if date is in the past', () => {
        var date = new Date();
        date.setMonth(date.getMonth() - 1);
        var result = addApprentiecships.calculateFundingCap(date, model);
        expect(result).toBeUndefined()
    });

});