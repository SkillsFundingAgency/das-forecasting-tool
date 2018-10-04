using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using BrowserStack;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests
{
    public class BrowserStackDriver
    {
        private IWebDriver driver;
        private Local browserStackLocal;
        private string profile;
        private string environment;
        private ScenarioContext context;

        public BrowserStackDriver(ScenarioContext context)
        {
            this.context = context;
        }

        public IWebDriver Init(string profile, string environment)
        {
            bool useBrowserStack = Boolean.Parse(ConfigurationManager.AppSettings["useBrowserStack"]);
            if (useBrowserStack)
            {
                NameValueCollection caps = ConfigurationManager.GetSection("capabilities/" + profile) as NameValueCollection;
                NameValueCollection settings = ConfigurationManager.GetSection("environments/" + environment) as NameValueCollection;

                DesiredCapabilities capability = new DesiredCapabilities();

                foreach (string key in caps.AllKeys)
                {
                    capability.SetCapability(key, caps[key]);
                }

                foreach (string key in settings.AllKeys)
                {
                    capability.SetCapability(key, settings[key]);
                }

                String username = Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME");
                if (username == null)
                {
                    username = ConfigurationManager.AppSettings.Get("user");
                }

                String accesskey = Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY");
                if (accesskey == null)
                {
                    accesskey = ConfigurationManager.AppSettings.Get("key");
                }

                capability.SetCapability("browserstack.user", username);
                capability.SetCapability("browserstack.key", accesskey);

                if (capability.GetCapability("browserstack.local") != null && capability.GetCapability("browserstack.local").ToString() == "true")
                {
                    browserStackLocal = new Local();
                    List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>()
                        { new KeyValuePair<string, string>("key", accesskey)};
                    browserStackLocal.start(bsLocalArgs);
                }

                capability.SetCapability("build", $"pp-das-fcast {DateTime.UtcNow.Year}/{DateTime.UtcNow.Month}/{DateTime.UtcNow.Day}");
                capability.SetCapability("name", TestContext.CurrentContext.Test.Name);
                driver = new RemoteWebDriver(new Uri("http://" + ConfigurationManager.AppSettings.Get("server") + "/wd/hub/"), capability);
                return driver;
            }

            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            return driver;
        }

        public IWebDriver GetExisting()
        {
            return driver;
        }

        public void Cleanup()
        {
            driver.Quit();
            if (browserStackLocal != null)
            {
                browserStackLocal.stop();
            }
        }
    }
}