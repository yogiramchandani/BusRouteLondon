using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace BusRouteLondon.Web.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "BusStopApi",
                routeTemplate: "api/{controller}/{lat}/{lng}/{radius}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiGet",
                routeTemplate: "Api/{controller}",
                defaults: new { action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
        }
    }
}
