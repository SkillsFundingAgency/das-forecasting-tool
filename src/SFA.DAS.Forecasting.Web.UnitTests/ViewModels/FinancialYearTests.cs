using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.ViewModels;
using System;

namespace SFA.DAS.Forecasting.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class FinancialYearTests
    {
        [TestCase("2008-02-01", 2007)]
        [TestCase("2003-03-01", 2002)]
        [TestCase("2020-01-15", 2019)]
        public void StartYear_should_be_previous_year_end_date_this_year(DateTime date, int expectedYear)
        {
            var fy = new FinancialYear(date);

            fy.StartDate.Year.Should().Be(expectedYear);
            fy.EndDate.Year.Should().Be(expectedYear + 1);
        }

        [TestCase("2008-04-01", 2008)]
        [TestCase("2003-09-01", 2003)]
        [TestCase("2020-12-15", 2020)]
        public void StartYear_should_be_this_year_end_year_next_year(DateTime date, int expectedYear)
        {
            var fy = new FinancialYear(date);

            fy.StartDate.Year.Should().Be(expectedYear);
            fy.EndDate.Year.Should().Be(expectedYear + 1);
        }
    }
}
