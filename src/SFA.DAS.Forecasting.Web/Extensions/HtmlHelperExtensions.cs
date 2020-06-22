using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.Models.Links;
using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString AddErrorClass<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return MvcHtmlString.Empty;
            }

            return new MvcHtmlString("error");
        }

        public static MvcHtmlString SetZenDeskLabels(this HtmlHelper html, params string[] labels)
        {   
            var keywords = string.Join(",", labels
                          .Where(label => !string.IsNullOrEmpty(label))
                          .Select(label => $"'{EscapeApostrophes(label)}'"));

            // when there are no keywords default to empty string to prevent zen desk matching articles from the url
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                + "] });</script>";

            return MvcHtmlString.Create(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }

        public static string GetZenDeskSnippetKey(this HtmlHelper html)
        {
            var forecastingConfig = DependencyResolver.Current.GetService<ForecastingConfiguration>();
            return forecastingConfig.ZenDeskSnippetKey;
        }

        public static string GetZenDeskSnippetSectionId(this HtmlHelper html)
        {
            var forecastingConfig = DependencyResolver.Current.GetService<ForecastingConfiguration>();
            return forecastingConfig.ZenDeskSectionId;
        }

        public static string GetZenDeskCobrowsingSnippetKey(this HtmlHelper html)
        {
            var forecastingConfig = DependencyResolver.Current.GetService<ForecastingConfiguration>();
            return forecastingConfig.ZenDeskCobrowsingSnippetKey;
        }

        private static string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["MyaBaseUrl"].EndsWith("/")
                ? ConfigurationManager.AppSettings["MyaBaseUrl"]
                : ConfigurationManager.AppSettings["MyaBaseUrl"] + "/";
        }

        public static IHeaderViewModel GetHeaderViewModel(this HtmlHelper html)
        {
            var configuration = DependencyResolver.Current.GetService<IStartupConfiguration>();
            var forecastingConfig = DependencyResolver.Current.GetService<ForecastingConfiguration>();
            var baseUrl = GetBaseUrl();
            var applicationBaseUrl = new Uri(html.ViewContext.HttpContext.Request.Url?.AbsoluteUri).AbsoluteUri.Replace(new System.Uri(html.ViewContext.HttpContext.Request.Url?.AbsoluteUri).AbsolutePath, "");

            var headerModel = new HeaderViewModel(new HeaderConfiguration
                {
                    EmployerCommitmentsBaseUrl = baseUrl,
                    EmployerFinanceBaseUrl = baseUrl,
                    ManageApprenticeshipsBaseUrl = baseUrl,
                    EmployerRecruitBaseUrl = forecastingConfig.EmployerRecruitBaseUrl,
                    AuthenticationAuthorityUrl = configuration.Identity.BaseAddress,
                    ClientId = configuration.Identity.ClientId,
                    SignOutUrl = new Uri($"{applicationBaseUrl}/forecasting/Service/signout")
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["hashedAccountId"]?.ToString()
                },
                useLegacyStyles: true);

            headerModel.SelectMenu(html.ViewBag.Section);

            if (html.ViewBag.HideNav != null && html.ViewBag.HideNav)
            {
                headerModel.HideMenu();
            }

            if (html.ViewData.Model?.GetType().GetProperty("HideHeaderSignInLink") != null)
            {
                headerModel.RemoveLink<SignIn>();
            }

            return headerModel;
        }

        public static IFooterViewModel GetFooterViewModel(this HtmlHelper html)
        {
            return new FooterViewModel(new FooterConfiguration
                {
                    ManageApprenticeshipsBaseUrl = GetBaseUrl()
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["HashedAccountId"]?.ToString()
                },
                useLegacyStyles: true);
        }

        public static ICookieBannerViewModel GetCookieBannerViewModel(this HtmlHelper html)
        {
            return new CookieBannerViewModel(new CookieBannerConfiguration
                {
                    ManageApprenticeshipsBaseUrl = GetBaseUrl()
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["HashedAccountId"]?.ToString()
                });
        }
    }
}