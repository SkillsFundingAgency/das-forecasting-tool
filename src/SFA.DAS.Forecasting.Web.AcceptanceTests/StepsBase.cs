using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    [Binding]
    public class StepsBase
    {
        protected static readonly string IisExpressPath = "C:\\Program Files (x86)\\IIS Express\\iisexpress.exe";
        protected static readonly string ApplicationHostConfigPath = $"..\\..\\..\\..\\.vs\\config\\applicationhost.config";
        protected static Config Config => ParentContainer.GetInstance<Config>();
        protected static List<Process> Processes = new List<Process>();
        protected static IContainer ParentContainer { get; set; }
        protected IContainer NestedContainer { get => Get<IContainer>(); set => Set(value); }
        protected ForecastingWebSite WebSite { get => Get<ForecastingWebSite>(); set => Set(value); }
        protected string EmployerHash { get => Get<string>("employer_hash"); set => Set(value, "employer_hash"); }
        protected string EmployeeLogin { get => Get<string>("employer_login"); set => Set(value, "employer_login"); }
        protected string EmployeePassword { get => Get<string>("employer_password"); set => Set(value, "employer_password"); }

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
            return Path.GetFullPath(Path.Combine(path, $"..\\..\\..\\..\\{appName}"));
        }

        private static string GetApiPath(string apiName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var path = codebase.LocalPath;
            return Path.GetFullPath(Path.Combine(path, $"..\\..\\..\\..\\{apiName}"));
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

        protected static void StartApi(string apiName)
        {
            if (!Config.Environment.Equals("DEV", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Can only start the api in dev environment.");
                return;
            }

            Console.WriteLine($"Starting the iis express. Path: {IisExpressPath}");
            var apiPath = GetApiPath(apiName);
            Console.WriteLine($"Api path: {apiPath}");
            var process = new Process
            {
                StartInfo =
                {
                    FileName = IisExpressPath,
                    Arguments = $"/config:{GetPath(ApplicationHostConfigPath)} /site:{apiName}",
                    WorkingDirectory = apiPath,
                    //UseShellExecute = true,
                }
            };
            process.Start();
            Processes.Add(process);
            Console.WriteLine("Giving the api time to start.");
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}