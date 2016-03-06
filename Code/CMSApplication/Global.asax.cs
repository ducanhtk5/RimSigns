using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMSApplication
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //About Us
            routes.MapRoute(
            "AboutUS", // Route name
            "AboutUS", // URL with parameters
            new { controller = "Common", action = "AboutUS" } // Parameter defaults
        );
            // Map Topic Detail
            routes.MapRoute(
                "TopicDetail", // Route name
                "Detail/{systemname}", // URL with parameters
                new { controller = "Topic", action = "Detail", systemname = UrlParameter.Optional }
            );

            routes.MapRoute(
                "TopicDetail2", // Route name
                "Topic/Detail/{systemname}", // URL with parameters
                new { controller = "Topic", action = "Detail", systemname = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }// Parameter defaults
            );

        }
        public static void RegisterViewEngine(ViewEngineCollection viewEngines)
        {
            // We do not need the default view engine
            viewEngines.Clear();

            var themeableRazorViewEngine = new ThemeableRazorViewEngine
            {
                CurrentTheme = httpContext => httpContext.Session["theme"] as string ?? string.Empty
            };

            viewEngines.Add(themeableRazorViewEngine);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            CMS.Service.Common.SendEmail.StartThreadSendMail();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterViewEngine(ViewEngines.Engines);
        }
    }
}