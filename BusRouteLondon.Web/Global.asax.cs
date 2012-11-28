using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BusrRouteLondon.Web.Controllers;
using BusrRouteLondon.Web.Infrastructure.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace BusRouteLondon.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : HttpApplication
    {
        public WebApiApplication()
        {
            BeginRequest += (sender, args) =>
                                {
                                    HttpContext.Current.Items[RavenController.CurrentRequestRavenSession] = RavenController.DocumentStore.OpenSession();
                                };
            EndRequest += (sender, args) =>
                              {
                                  using (var session = (IDocumentSession) HttpContext.Current.Items[RavenController.CurrentRequestRavenSession])
                                  {
                                      if (session == null)
                                      {
                                          return;
                                      }
                                      if (Server.GetLastError() != null)
                                      {
                                          return;
                                      }
                                      session.SaveChanges();
                                  }
                              };
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            InitialiseDocumentStore();
            RavenController.DocumentStore = DocumentStore;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static IDocumentStore DocumentStore { get; private set; }

        private static void InitialiseDocumentStore()
        {
            if (DocumentStore != null) return;
            DocumentStore = new DocumentStore
                                {
                                    ConnectionStringName = "RavenDB"
                                }.Initialize();
            TryCreatingIndexesOrRedirectToErrorPage();
        }

        private static void TryCreatingIndexesOrRedirectToErrorPage()
        {
            try
            {
                IndexCreation.CreateIndexes(typeof(BusStop_BusStopCode).Assembly, DocumentStore);
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