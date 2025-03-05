using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Extensions;
using System;

namespace SFA.DAS.Forecasting.Web.UnitTests;

[TestFixture]
public class DateTimeExtensionsTests
{
    [TestCase("2018-04-30", "2018-04-30")]
    [TestCase("2018-04-19", "2018-04-04")]
    [TestCase("2018-04-04", "2018-04-19")]
    [TestCase("2018-05-01", "2018-04-30")]
    public void DateTimeIsAfter(DateTime dt1, DateTime dt2)
    {
        dt1.IsAfterOrSameMonth(dt2).Should().BeTrue();
    }

    [TestCase("2018-01-01", "2018-05-01")]
    [TestCase("2017-05-01", "2018-05-01")]
    public void DateTimeIsBefore(DateTime dt1, DateTime dt2)
    {
        dt1.IsAfterOrSameMonth(dt2).Should().BeFalse();
    }
}