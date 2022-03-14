﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.SqlClient;
using System.Web.Http;

namespace GNT_server
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }

            ); 
             config.Routes.MapHttpRoute(
                 name: "PutApi",
                 routeTemplate: "api/{controller}/{id}",
                 defaults: new { id = RouteParameter.Optional }

             );
            config.Routes.MapHttpRoute(
                name: "PostApi",
                routeTemplate: "api/{controller}",
                defaults: new { id = RouteParameter.Optional }

            );
        }
    }
}
