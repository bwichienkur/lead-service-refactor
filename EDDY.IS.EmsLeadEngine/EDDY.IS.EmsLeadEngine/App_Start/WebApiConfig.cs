using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using System.Web.Http;

namespace EDDY.IS.EmsLeadEngine
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
           GlobalConfiguration.Configuration.MessageHandlers.Insert(0, new ServerCompressionHandler(new System.Net.Http.Extensions.Compression.Core.Compressors.GZipCompressor(), new DeflateCompressor()));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
