using BusRouteLondon.Web.App_Start;
using BusRouteLondon.Web.Infrastructure.Indexes;
using Raven.Client;
using Raven.Client.Indexes;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BusRouteLondon.Web
{
    public class WebApiApplication : HttpApplication
    {
        private static IDocumentStore DocumentStore
        {
            get { return DependencyResolver.Current.GetService<IDocumentStore>(); }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            HttpFilterConfig.RegisterHttpFilters(GlobalConfiguration.Configuration.Filters, DocumentStore);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            TryCreatingIndexesOrRedirectToErrorPage(DocumentStore);
        }

        private static void TryCreatingIndexesOrRedirectToErrorPage(IDocumentStore store)
        {
            try
            {
                IndexCreation.CreateIndexes(typeof(BusStop_Spatial).Assembly, store);
            }
            catch (WebException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null)
                    throw;

                switch (socketException.SocketErrorCode)
                {
                    case SocketError.AddressNotAvailable:
                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                    case SocketError.TimedOut:
                    case SocketError.ConnectionRefused:
                    case SocketError.HostDown:
                    case SocketError.HostUnreachable:
                    case SocketError.HostNotFound:
                        HttpContext.Current.Response.Redirect("~/DBNotReachable.htm");
                        break;
                    default:
                        throw;
                }
            }
        }
    }
}