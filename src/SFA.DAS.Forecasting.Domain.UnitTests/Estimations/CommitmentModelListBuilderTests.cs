using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations;

[TestFixture]
public class CommitmentModelListBuilderTests
{
    private List<VirtualApprenticeship> _virtualApprenticeships;
    private CommitmentModelListBuilder _commitmentsModelListBuilder;

    [SetUp]
    public void SetUp()
    {
        _virtualApprenticeships = new List<VirtualApprenticeship>
        {
            new()
            {
                ApprenticesCount = 2,
                CourseId = "7",
                CourseTitle = "Josevi driver",
                Id = "1",
                Level = 10,
                StartDate = DateTime.Today,
                TotalCompletionAmount = 2000,
                TotalCost = 10000,
                TotalInstallmentAmount = 120,
                TotalInstallments = 12,
                FundingSource = Models.Payments.FundingSource.Transfer
            },
            new()
            {
                ApprenticesCount = 2,
                CourseId = "7",
                CourseTitle = "Josevi driver 2",
                Id = "2",
                Level = 10,
                StartDate = DateTime.Today.AddMonths(13),
                TotalCompletionAmount = 100,
                TotalCost = 500,
                TotalInstallmentAmount = 300,
                TotalInstallments = 12,
                FundingSource = 0
            }
        };
        _commitmentsModelListBuilder = new CommitmentModelListBuilder();
    }

    [Test]
    public void Modelled_Apprentices_Are_Created_As_Sender_Apprentices()
    {
        var modelledApprenticeships = _commitmentsModelListBuilder.Build(12345, _virtualApprenticeships);
        Assert.IsTrue(modelledApprenticeships.All(modelledApprenticeship => modelledApprenticeship.SendingEmployerAccountId == 12345 && modelledApprenticeship.EmployerAccountId == 0));
    }

    [Test]
    public void Creates_Correct_Number_Of_Modelled_Apprentices_For_The_Virtual_Apprenticeships()
    {
        Assert.AreEqual(_virtualApprenticeships.Sum(va => va.ApprenticesCount),
            _commitmentsModelListBuilder.Build(12345, _virtualApprenticeships).Count);
    }

    [Test]
    public void Calculates_Correct_Completion_Payments()
    {
        var modelledApprenticeships = _commitmentsModelListBuilder.Build(12345, _virtualApprenticeships);
        _virtualApprenticeships.ForEach(va =>
        {
            modelledApprenticeships.Where(ma => ma.CourseName == va.CourseTitle).ToList().ForEach(ma => Assert.AreEqual(va.TotalCompletionAmount / va.ApprenticesCount,ma.CompletionAmount));
        });
    }

    [Test]
    public void Calculates_Correct_Installment_Amount()
    {
        var modelledApprenticeships = _commitmentsModelListBuilder.Build(12345, _virtualApprenticeships);
        _virtualApprenticeships.ForEach(va =>
        {
            modelledApprenticeships.Where(ma => ma.CourseName == va.CourseTitle).ToList().ForEach(ma => Assert.AreEqual(va.TotalInstallmentAmount / va.ApprenticesCount, ma.MonthlyInstallment));
        });
    }
}