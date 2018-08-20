using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Apprenticeship;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure;
using SFA.DAS.Forecasting.AcceptanceTests.Levy;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using SFA.DAS.Forecasting.Application.Commitments.Services;
using SFA.DAS.Forecasting.Application.Converters;
using SFA.DAS.Forecasting.Application.Infrastructure.Persistence;
using SFA.DAS.Forecasting.Application.Infrastructure.Registries;
using SFA.DAS.Forecasting.Application.Shared;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Levy;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    [Binding]
    public class StepsBase
    {
        protected static IContainer ParentContainer { get; set; }

        protected static Config Config => ParentContainer.GetInstance<Config>();
        protected static readonly string FunctionsToolsRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AzureFunctionsTools", "Releases");
        protected static string FunctionsToolsPath => Path.Combine(FunctionsToolsRootPath, GetAzureFunctionsToolsVersion(), "cli", "func.exe");
        protected static readonly string FunctionsCliRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Azure.Functions.Cli");
        protected static string FunctionsCliPath => Path.Combine(FunctionsCliRootPath, GetAzureFunctionsCliVersion(), "func.exe");
        protected static string FunctionsPath => Directory.Exists(FunctionsToolsRootPath) ? FunctionsToolsPath : FunctionsCliPath;

        protected IContainer NestedContainer { get => Get<IContainer>(); set => Set(value); }
        protected IDbConnection Connection => NestedContainer.GetInstance<IDbConnection>();
        protected ForecastingDataContext DataContext => NestedContainer.GetInstance<ForecastingDataContext>();
        protected IDocumentSession Session => NestedContainer.GetInstance<IDocumentSession>();

        protected CommitmentsDataService CommitmentsDataService => NestedContainer.GetInstance<CommitmentsDataService>();
        protected string EmployerHash { get => Get<string>("employer_hash"); set => Set(value, "employer_hash"); }
        protected static List<Process> Processes = new List<Process>();
        protected int EmployerAccountId => Config.EmployerAccountId;
        protected List<TestPayment> Payments { get => Get<List<TestPayment>>(); set => Set(value); }
        protected PayrollPeriod PayrollPeriod { get => Get<PayrollPeriod>(); set => Set(value); }
        protected List<LevySubmission> LevySubmissions { get => Get<List<LevySubmission>>(); set => Set(value); }
        protected List<TestApprenticeship> TestApprenticeships { get => Get<List<TestApprenticeship>>(); set => Set(value); }

        protected List<TestCommitment> Commitments { get => Get<List<TestCommitment>>(); set => Set(value); }
        protected List<AccountProjectionModel> AccountProjections { get => Get<List<AccountProjectionModel>>(); set => Set(value); }
        protected List<Models.Payments.PaymentModel> RecordedPayments { get => Get<List<Models.Payments.PaymentModel>>(); set => Set(value); }
        protected List<Models.Commitments.CommitmentModel> RecordedCommitments { get => Get<List<Models.Commitments.CommitmentModel>>(); set => Set(value); }
        protected CalendarPeriod ProjectionsStartPeriod
        {
            get => Get<CalendarPeriod>("projections_start_period");
            set => Set(value, "projections_start_period");
        }
        protected decimal Balance
        {
            get => (decimal)Get<object>("current_balance");
            set => Set(value, "current_balance");
        }

        protected bool IsTransfer { get => Get<bool>("is_transfer"); set => Set(value, "is_transfer"); }
        protected CommitmentType CommitmentType { get => Get<CommitmentType>(); set => Set(value); }

        protected static string GetAzureFunctionsToolsVersion()
        {
            return Directory.GetDirectories(FunctionsToolsRootPath)
                .Select(directoryI => new DirectoryInfo(directoryI))
                .Select(directoryInfo => directoryInfo.Name)
                .ToList()
                .Where(c => Convert.ToInt32(c.Split('.').First()) == 1)
                .OrderByDescending(c => Convert.ToInt32(c.Split('.')[0]))
                .ThenByDescending(c => Convert.ToInt32(c.Split('.')[1]))
                .ThenByDescending(c => Convert.ToInt32(c.Split('.')[2]))
                .First();
        }

        protected static string GetAzureFunctionsCliVersion()
        {
            return Directory.GetDirectories(FunctionsCliRootPath)
                .Select(directoryI => new DirectoryInfo(directoryI))
                .Select(directoryInfo => directoryInfo.Name)
                .ToList()
                .OrderByDescending(c => Convert.ToInt32(c.Split('.')[0]))
                .ThenByDescending(c => Convert.ToInt32(c.Split('.')[1]))
                .ThenByDescending(c => Convert.ToInt32(c.Split('.')[2]))
                .First();
        }

        protected static HttpClient HttpClient = new HttpClient();

        public T Get<T>(string key = null)// where T : class
        {
            return key == null ? ScenarioContext.Current.Get<T>() : ScenarioContext.Current.Get<T>(key);
        }

        public void Set<T>(T item, string key = null)
        {
            if (key == null)
                ScenarioContext.Current.Set(item);
            else
                ScenarioContext.Current.Set(item, key);
        }

        protected void WaitForIt(Func<bool> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            while (DateTime.Now < endTime)
            {
                if (lookForIt())
                    return;
                Thread.Sleep(Config.TimeToPause);
            }
            Assert.Fail(failText);
        }

        protected void WaitForIt(Func<Tuple<bool, string>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var reason = "";
            var pass = false;
            while (DateTime.Now < endTime)
            {
                (pass, reason) = lookForIt();
                if (pass)
                    return;
                Thread.Sleep(Config.TimeToPause);
            }
            Assert.Fail(failText + " - " + reason);
        }

        protected bool WaitForIt(Func<bool> lookForIt)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            while (DateTime.Now < endTime)
            {
                if (lookForIt())
                    return true;
                Thread.Sleep(Config.TimeToPause);
            }
            return false;
        }

        private static string GetAppPath(string appName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = codebase.LocalPath;
            return Path.GetFullPath(Path.Combine(path, $"..\\..\\..\\..\\{appName}\\bin\\Debug\\net462"));
        }

        private static string GetLocalPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            return codebase.LocalPath;
        }

        protected static string GetPath(string pathPart)
        {
            return Path.Combine(GetLocalPath(), pathPart);
        }

        protected static void StartFunction(string functionName)
        {
            if (!Config.IsDevEnvironment)
            {
                Console.WriteLine("Can only start the function in dev environment.");
                return;
            }

            Console.WriteLine($"Starting the function cli. Path: {FunctionsPath}");
            var appPath = GetAppPath(functionName);
            Console.WriteLine($"Function path: {appPath}");
            if (!Directory.Exists(appPath))
            {
                throw new Exception($"Function path: {appPath} path does not exist");
            }

            var process = new Process
            {
                StartInfo =
                {
                    FileName = FunctionsPath,
                    Arguments = $"host start",
                    WorkingDirectory = appPath,
                    //UseShellExecute = true,
                }
            };
            process.Start();
            Processes.Add(process);
            Console.WriteLine("Giving the function time to start.");
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        protected void DeleteLevyDeclarations()
        {
            DeleteLevyDeclarations(PayrollPeriod.PayrollYear, PayrollPeriod.PayrollMonth);
        }

        protected void DeleteLevyDeclarations(string payrollYear, short payrollMonth)
        {
            var levyDeclarations = DataContext.LevyDeclarations
                .Where(levy => levy.EmployerAccountId == Config.EmployerAccountId &&
                               levy.PayrollMonth == payrollMonth &&
                               levy.PayrollYear == payrollYear)
                .ToList();
            DataContext.LevyDeclarations.RemoveRange(levyDeclarations);
            DataContext.SaveChanges();
        }

        protected void DeleteCommitments(long employerId)
        {
            var commitments = DataContext.Commitments
                .Where(c => c.EmployerAccountId == employerId || c.SendingEmployerAccountId == employerId)
                .ToList();
            DataContext.Commitments.RemoveRange(commitments);
            DataContext.SaveChanges();
        }

        protected void DeleteBalance()
        {
            var balance = DataContext.Balances.FirstOrDefault(b => b.EmployerAccountId == EmployerAccountId);
            if (balance == null)
                return;
            DataContext.Balances.Remove(balance);
            DataContext.SaveChanges();
        }

        protected void DeleteAccountProjections(long employerId)
        {
            var projections = DataContext.AccountProjections
                .Where(projection => projection.EmployerAccountId == employerId)
                .ToList();
            DataContext.AccountProjections.RemoveRange(projections);
            DataContext.SaveChanges();
            RemoveEmployerProjectionAuditDocuments(ProjectionSource.PaymentPeriodEnd, employerId);
            RemoveEmployerProjectionAuditDocuments(ProjectionSource.LevyDeclaration, employerId);
        }

        protected void InsertLevyDeclarations(IEnumerable<LevySubmission> levySubmissions)
        {
            InsertLevyDeclarations(PayrollPeriod, levySubmissions);
        }

        protected void InsertLevyDeclarations(PayrollPeriod period, IEnumerable<LevySubmission> levySubmissions)
        {
            var payrollDate = new PayrollDateService().GetPayrollDate(period.PayrollYear, period.PayrollMonth);
            foreach (var levySubmission in levySubmissions)
            {
                DataContext.LevyDeclarations.Add(new LevyDeclarationModel
                {
                    EmployerAccountId = Config.EmployerAccountId,
                    DateReceived = DateTime.Now,
                    LevyAmountDeclared = levySubmission.Amount,
                    PayrollDate = payrollDate,
                    PayrollMonth = (byte)period.PayrollMonth,
                    PayrollYear = period.PayrollYear,
                    Scheme = levySubmission.Scheme,
                    TransactionDate = levySubmission.CreatedDateValue
                });
            }

            DataContext.SaveChanges();
        }

        protected void InsertCommitments(List<TestCommitment> commitments)
        {
            var senderId = CommitmentType == CommitmentType.TransferReceiver ? 54321 : EmployerAccountId;
            var receiverId = CommitmentType == CommitmentType.TransferSender ? 54321 : EmployerAccountId;

            for (var i = 0; i < commitments.Count; i++)
            {
                var commitment = commitments[i];

                var isTransferSender = CommitmentType == CommitmentType.TransferSender;
                var isFundingSourceLevy = commitment.FundingSource == FundingSource.Levy;

                var commitmentModel = new CommitmentModel
                {
                    EmployerAccountId = isTransferSender && isFundingSourceLevy ? EmployerAccountId : receiverId,
                    LearnerId = i + 1,
                    ApprenticeshipId = commitment.ApprenticeshipId > 0 ? commitment.ApprenticeshipId : i + 2,
                    ApprenticeName = commitment.ApprenticeName,
                    SendingEmployerAccountId = senderId,
                    ProviderId = i + 3,
                    ProviderName = commitment.ProviderName,
                    CourseName = commitment.CourseName,
                    CourseLevel = commitment.CourseLevel,
                    StartDate = commitment.StartDateValue,
                    PlannedEndDate = commitment.PlannedEndDate,
                    ActualEndDate = commitment.ActualEndDateValue.Value == DateTime.MinValue ? null : commitment.ActualEndDateValue,
                    CompletionAmount = commitment.CompletionAmount,
                    MonthlyInstallment = commitment.InstallmentAmount,
                    NumberOfInstallments = (short)commitment.NumberOfInstallments,
                    FundingSource = GetFundingSource(commitment)
                };
                CommitmentsDataService.Upsert(commitmentModel).Wait();
            }

            DataContext.SaveChanges();
        }

        private FundingSource GetFundingSource(TestCommitment commitment)
        {
            return commitment.FundingSource != 0 ? commitment.FundingSource : FundingSource.Levy;
        }

        protected void ExecuteSql(Action action)
        {
            using (var txScope = new TransactionScope())
            {
                action();
                txScope.Complete();
            }
        }

        protected void ExecuteSql(Action<TransactionScope> action)
        {
            using (var txScope = new TransactionScope())
            {
                action(txScope);
                txScope.Complete();
            }
        }

        protected void Send<T>(string endpoint, T payload, string description = "Posting content to api.")
        {
            Send(endpoint, JsonConvert.SerializeObject(payload), description);
        }

        protected void Send(string endpoint, string payload, string description = "Posting content to api.")
        {
            Console.WriteLine($"{description} Uri: {endpoint}, Payload: {payload}");
            HttpClient.PostAsync(endpoint, new StringContent(payload)).Wait();
        }

        protected void RemoveEmployerProjectionAuditDocuments(ProjectionSource projectionSource, params long[] employerAccountIds)
        {
            RemoveEmployerProjectionAuditDocuments(Session, projectionSource, employerAccountIds);
        }

        protected void RemoveEmployerProjectionAuditDocuments(IDocumentSession session, ProjectionSource projectionSource, params long[] employerAccountIds)
        {
            foreach (var employerAccountId in employerAccountIds)
            {
                var docId = $"employerprojectionaudit-{projectionSource.ToString("G").ToLower()}-{employerAccountId}";  //TODO: should really be using a common bit of code here
                session.Delete(docId).Wait();
            }
        }

    }
}