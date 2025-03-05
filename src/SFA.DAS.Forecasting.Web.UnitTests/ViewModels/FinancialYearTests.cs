using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.UnitTests.ViewModels;

[TestFixture]
public class FinancialYearTests
{
    [TestCase("2008-02-01", 2007, Description = "If date before April start year must be previous year")]
    [TestCase("2003-03-01", 2002, Description = "If date before April start year must be previous year")]
    [TestCase("2020-01-15", 2019, Description = "If date before April start year must be previous year")]
    [TestCase("2008-04-01", 2008, Description = "If date after or on April start year must be this year")]
    [TestCase("2003-09-01", 2003, Description = "If date after or on April start year must be this year")]
    [TestCase("2020-12-15", 2020, Description = "If date after or on April start year must be this year")]
    public void Then_The_Year_Is_Correctly_Calculated_For_The_Financial_Year(DateTime date, int expectedYear)
    {
        var fy = new FinancialYear(date);

        fy.StartDate.Year.Should().Be(expectedYear);
        fy.EndDate.Year.Should().Be(expectedYear + 1);
    }
}