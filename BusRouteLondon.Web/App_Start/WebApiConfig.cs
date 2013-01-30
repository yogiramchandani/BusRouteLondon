using System.Web.Http;

namespace BusRouteLondon.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "BusStopApi",
                routeTemplate: "api/{controller}/{lat}/{lng}/{radius}"
            );
        }
    }
}
