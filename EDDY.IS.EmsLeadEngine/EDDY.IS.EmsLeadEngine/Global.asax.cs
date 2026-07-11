using EDDY.IS.EmsLeadEngine.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace EDDY.IS.EmsLeadEngine
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            CacheDataService.Initialize();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
