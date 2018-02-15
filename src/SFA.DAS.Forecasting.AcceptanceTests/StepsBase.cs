using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Transactions;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure;
using SFA.DAS.Forecasting.AcceptanceTests.Levy;
using SFA.DAS.Forecasting.AcceptanceTests.Payments;
using SFA.DAS.Forecasting.Application.Shared;
using SFA.DAS.Forecasting.ReadModel.Projections;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests
{
    [Binding]
    public class StepsBase
    {
        protected static IContainer ParentContainer { get; set; }

        protected static Config Config => ParentContainer.GetInstance<Config>();
        protected static readonly string FunctionsCliPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Azure.Functions.Cli", "1.0.8", "func.exe");
        protected IContainer NestedContainer { get => Get<IContainer>(); set => Set(value); }
        protected IDbConnection Connection => NestedContainer.GetInstance<IDbConnection>();
        protected string EmployerHash { get => Get<string>("employer_hash"); set => Set(value, "employer_hash"); }
        protected static List<Process> Processes = new List<Process>();
        protected int EmployerAccountId => Config.EmployerAccountId;
        protected PayrollPeriod PayrollPeriod { get => Get<PayrollPeriod>(); set => Set(value); }
        protected List<LevySubmission> LevySubmissions { get => Get<List<LevySubmission>>(); set => Set(value); }
        protected List<TestCommitment> Commitments { get => Get<List<TestCommitment>>(); set => Set(value); }
        protected List<AccountProjectionReadModel> AccountProjections  { get => Get<List<AccountProjectionReadModel>>(); set => Set(value); }

        protected decimal Balance
        {
            get => (decimal)Get<object>("current_balance");
            set => Set(value, "current_balance");
        }

        protected static HttpClient HttpClient = new HttpClient();
        public T Get<T>(string key = null) where T : class
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
            if (!Config.Environment.Equals("DEV", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Can only start the function in dev environment.");
                return;
            }

            Console.WriteLine($"Starting the function cli. Path: {FunctionsCliPath}");
            var appPath = GetAppPath(functionName);
            Console.WriteLine($"Function path: {appPath}");
            var process = new Process
            {
                StartInfo =
                {
                    FileName = FunctionsCliPath,
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
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            parameters.Add("@payrollYear", payrollYear, DbType.String);
            parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);
            Connection.Execute("Delete from LevyDeclaration where employerAccountId = @employerAccountId and PayrollYear = @payrollYear and PayrollMonth = @payrollMonth",
                parameters, commandType: CommandType.Text);
        }

        protected void DeleteCommitments()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from Commitment where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text);
        }

        protected void DeleteBalance()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from Balance where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text);
        }

        protected void DeleteAccountProjections()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from AccountProjection where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text);
        }

        protected void InsertNewBalance(decimal balance)
        {
            ExecuteSql(() =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                parameters.Add("@amount", balance, DbType.Decimal);
                parameters.Add("@balancePeriod", DateTime.Today, DbType.DateTime);
                parameters.Add("@receivedDate", DateTime.Today, DbType.DateTime);
                Connection.Execute(@"Insert into Balance values (@employerAccountId, @amount, @balancePeriod, @receivedDate)",
                    parameters, commandType: CommandType.Text);
            });
        }

        protected void InsertLevyDeclarations(IEnumerable<LevySubmission> levySubmissions)
        {
            InsertLevyDeclarations(PayrollPeriod, levySubmissions);
        }

        protected void InsertLevyDeclarations(PayrollPeriod period, IEnumerable<LevySubmission> levySubmissions)
        {
            var payrollDate = new PayrollDateService().GetPayrollDate(period.PayrollYear, period.PayrollMonth);
            ExecuteSql(() =>
            {
                foreach (var levySubmission in levySubmissions)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                    parameters.Add("@scheme", levySubmission.Scheme, DbType.String);
                    parameters.Add("@payrollYear", period.PayrollYear, DbType.String);
                    parameters.Add("@payrollMonth", period.PayrollMonth, DbType.Int16);
                    parameters.Add("@payrollDate", payrollDate, DbType.DateTime);
                    parameters.Add("@levyAmountDeclared", levySubmission.Amount, DbType.Decimal);
                    parameters.Add("@transactionDate", levySubmission.CreatedDateValue, DbType.DateTime);
                    parameters.Add("@dateReceived", DateTime.Now, DbType.DateTime);
                    Connection.Execute(@"Insert into LevyDeclaration values (@employerAccountId, @scheme, @payrollYear, @payrollMonth, @payrollDate, @levyAmountDeclared, @transactionDate, @dateReceived)",
                        parameters, commandType: CommandType.Text);
                }
            });
        }

        protected void InsertCommitments(List<TestCommitment> commitments)
        {
            ExecuteSql(() =>
            {
                for (int i = 0; i < commitments.Count; i++)
                {
                    var commitment = commitments[i];

                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
                    parameters.Add("@learnerId", i + 1, DbType.Int64);
                    parameters.Add("@apprenticeshipId", i + 2, DbType.Int64);
                    parameters.Add("@apprenticeName", commitment.ApprenticeName, DbType.String);
                    parameters.Add("@providerId", i + 3, DbType.Int64);
                    parameters.Add("@providerName", commitment.ProviderName, DbType.String);
                    parameters.Add("@courseName", commitment.CourseName, DbType.String);
                    parameters.Add("@courseLevel", commitment.CourseLevel, DbType.Int32);
                    parameters.Add("@startDate", commitment.StartDateValue, DbType.DateTime);
                    parameters.Add("@plannedEndDate", commitment.PlannedEndDate, DbType.DateTime);
                    parameters.Add("@actualEndDate", null, DbType.DateTime);
                    parameters.Add("@completionAmount", commitment.CompletionAmount, DbType.Decimal);
                    parameters.Add("@monthlyInstallment", commitment.InstallmentAmount, DbType.Decimal);
                    parameters.Add("@numberOfInstallments", commitment.NumberOfInstallments, DbType.Int16);
                    Connection.Execute(@"Insert into Commitment values (@employerAccountId, @learnerId, @apprenticeshipId, @apprenticeName, @providerId, @providerName,@courseName, @courseLevel, @startDate, @plannedEndDate, @actualEndDate,@completionAmount, @monthlyInstallment, @numberOfInstallments)",
                        parameters, commandType: CommandType.Text);
                }
            });
        }

        protected void ExecuteSql(Action action)
        {
            using (var txScope = new TransactionScope())
            {
                action();
                txScope.Complete();
            }
        }
    }
}
