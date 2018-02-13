using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    static class FileManager
    {
        public static string[] getCurrentDownloadFiles()
        {
            return Directory.GetFiles(FileManager.DownloadsFolderPath);
        }

        private static string DownloadsFolderPath
        {
            get
            {
                string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(user, "Downloads");
            }

            set { }
        }
    }
}
