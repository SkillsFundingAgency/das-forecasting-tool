'use strict';

var addApprentiecships = require('../dist/js/add-apprenticeships');

var model =
    {
        CourseId: 12,
        FundingBands: [
            { FromDate: "/Date(1514764800000)/", ToDate: "/Date(1546214400000)/", FundingCap: 100 },
            { FromDate: "/Date(1546300800000)/", ToDate: "/Date(1577836800000)/", FundingCap: 200 },
            { FromDate: "/Date(1577750400000)/", ToDate: 'null', FundingCap: 300 }
        ]
    }

describe('add apprenticeships', () => {

    it ('calculate funding cap', () => {
        var result = addApprentiecships.calculateFundingCap(new Date(2018, 1, 1), model);
        expect(result.FundingCap).toBe(100);
    });

    it('calculate funding cap when date is middle year', () => {
        var result = addApprentiecships.calculateFundingCap(new Date(2019, 1, 1), model);
        expect(result.FundingCap).toBe(200);
    });

    it('calculate funding cap when date is last year', () => {
        var result = addApprentiecships.calculateFundingCap(new Date(2020, 1, 1), model);
        expect(result.FundingCap).toBe(300);
    });

    it('calculate funding cap when date is after last funding band', () => {
        var result = addApprentiecships.calculateFundingCap(new Date(2022, 1, 1), model);
        expect(result.FundingCap).toBe(300);
    });


    it('is undefined if date is not valid', () => {
        var result = addApprentiecships.calculateFundingCap(new Date(-1, "abba", 1), model);
        expect(result).toBeUndefined()
    });

    it('is undefined if date is undefined', () => {
        var result = addApprentiecships.calculateFundingCap(undefined, model);
        expect(result).toBeUndefined()
    });

});