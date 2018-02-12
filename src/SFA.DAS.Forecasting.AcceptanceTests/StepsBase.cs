using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure.Registries;
using SFA.DAS.Forecasting.AcceptanceTests.Levy;
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
        protected IContainer NestedContainer { get => Get<IContainer>();  set => Set(value); }
        protected IDbConnection Connection => NestedContainer.GetInstance<IDbConnection>();
        protected string EmployerHash { get => Get<string>("employer_hash"); set => Set(value, "employer_hash"); }
        protected static List<Process> Processes = new List<Process>();
        protected int EmployerAccountId => Config.EmployerAccountId;
        protected PayrollPeriod PayrollPeriod { get => Get<PayrollPeriod>(); set => Set(value); }
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

    }
}
