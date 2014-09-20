using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System;

namespace Hite.Web.Controllers.Site
{
    //SWFUpload Document : http://demo.swfupload.org/Documentation/
    public class Global : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("style/{*pathInfo}");
            routes.IgnoreRoute("scripts/{*pathInfo}");
            routes.IgnoreRoute("images/{*pathInfo}");


            #region == Search ==
            routes.MapRoute("Search",
                "search.html",
                new { controller = "Home", action = "Search" }
                );
            routes.MapRoute("EnSearch",
                "en/search.html",
                new { controller = "Home", action = "Search" }
                );
            #endregion

            #region == Feedback ==
            routes.MapRoute("Feedback",
                "feedback.html",
                new { controller = "Home", action = "Feedback" }
                );
            routes.MapRoute("EnFeedback",
                "en/feedback.html",
                new { controller = "Home", action = "Feedback" }
                );
            #endregion

            #region == ContactUs ==
            routes.MapRoute("ContactUs",
                "contactus.html",
                new { controller = "Home", action = "ContactUs" }
            );
            routes.MapRoute("EnContactUs",
                "en/contactus.html",
                new { controller = "Home", action = "ContactUs" }
            );
            #endregion

            #region == Hr ==
            routes.MapRoute("Hr",
                "hr.html",
                new { controller = "Channel", action = "Hr" }
                );
            routes.MapRoute("EnHr",
                "en/hr.html",
                new { controller = "Channel", action = "Hr" }
                );
            #endregion

            #region == Static ==
            routes.MapRoute("static",
                "static/{action}",
                 new { controller = "Static", action = "js" });
            routes.MapRoute("en-static",
                "en/static/{action}",
                 new { controller = "Static", action = "js" });
            #endregion

            #region == Accounts ==
            routes.MapRoute("Accounts",
                "accounts/{action}",
                new { controller = "Account", action = "Index" }
                );
            routes.MapRoute("EnAccounts",
                "en/accounts/{action}",
                new { controller = "Account", action = "Index" }
                );
            #endregion

            #region == ArticleShow ==
            routes.MapRoute(
                "ArticleShow",
                "{year}-{month}-{day}/{id}.html",
                new { controller = "Article", action = "Detail", id = @"\d+", year = @"\d+", month = @"\d+", day = @"\d+", data = @"\d{0,4}-\d{0,2}-\d{0,2}" }
            );
            routes.MapRoute(
                "EnArticleShow",
                "en/{year}-{month}-{day}/{id}.html",
                new { controller = "Article", action = "Detail", id = @"\d+", year = @"\d+", month = @"\d+", day = @"\d+", data = @"\d{0,4}-\d{0,2}-\d{0,2}" }
            );
            #endregion

            #region == Channel ==
            routes.MapRoute(
                "Channel_default",
                "Channel/{cat}.html",
                new { controller = "Channel", action = "Show" }
            );
            routes.MapRoute(
                "EnChannel_default",
                "en/channel/{cat}.html",
                new { controller = "Channel", action = "Show" }
            );
            #endregion

            routes.MapRoute("404", "404.html", new { controller = "Home", action = "NotFound404" });



            routes.MapRoute(
                "EnDefault", // Route name
                "en/{action}", // URL with parameters
                new { controller = "Home", action = "Index" }
            );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Home", action = "Index"}
            );

        }
        
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            log4net.Config.XmlConfigurator.Configure();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
        void Application_BeginRequest(Object sender, EventArgs e) {
            //try
            //{
            //    if (Request.Cookies["language"] != null)
            //    {
            //        string _cookieValue = Request.Cookies["Language"].Value == "zh_cn" ? "zh-cn" : "en-us";
            //        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(_cookieValue);

            //        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(_cookieValue);
            //    }
            //}
            //catch (Exception){ }
        }
    }
}