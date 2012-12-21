using System;
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
    public class WebApiApplication : HttpApplication
    {
        private static readonly Lazy<IDocumentStore> _documentStore =
            new Lazy<IDocumentStore>(() =>
                                         {
                                             var docStore = new DocumentStore { ConnectionStringName = "RavenDB" };
                                             docStore.Initialize();
                                             TryCreatingIndexesOrRedirectToErrorPage();
                                             return docStore;
                                         });

        public static IDocumentStore DocumentStore
        {
            get { return _documentStore.Value; }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RavenController.DocumentStore = DocumentStore;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

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