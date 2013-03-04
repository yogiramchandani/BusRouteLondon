using BusRouteLondon.Web.Infrastructure.Filter;
using Raven.Client;
using System.Web.Http.Filters;

namespace BusRouteLondon.Web.App_Start
{
    public class HttpFilterConfig
    {
        public static void RegisterHttpFilters(HttpFilterCollection filters, IDocumentStore store)
        {
            filters.Add(new RavenSessionManagementAttribute(store));
        }
    }
}