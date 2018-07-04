using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SFA.DAS.Forecasting.PerformanceTests.Infrastructure
{
    public class FunctionFunner
    {
        protected static readonly string FunctionsToolsRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AzureFunctionsTools", "Releases");
        protected static string FunctionsToolsPath => Path.Combine(FunctionsToolsRootPath, GetAzureFunctionsToolsVersion(), "cli", "func.exe");
        protected static readonly string FunctionsCliRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Azure.Functions.Cli");
        protected static string FunctionsCliPath => Path.Combine(FunctionsCliRootPath, GetAzureFunctionsCliVersion(), "func.exe");
        protected static string FunctionsPath => Directory.Exists(FunctionsToolsRootPath) ? FunctionsToolsPath : FunctionsCliPath;

        protected List<Process > Processes = new List<Process>();

        protected static string GetAzureFunctionsToolsVersion()
        {
            return Directory.GetDirectories(FunctionsToolsRootPath)
                .Select(directoryI => new DirectoryInfo(directoryI))
                .Select(directoryInfo => directoryInfo.Name)
                .ToList()
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
        public void StartFunction(string functionName)
        {
            if (!Config.Instance.IsDevEnvironment)
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

        public void StopFunctions()
        {
            Processes?.ForEach(process =>
            {
                var processName = process.ProcessName;
                if (!process.HasExited)
                    process.Kill();
            });
            Processes?.Clear();
        }
    }
}