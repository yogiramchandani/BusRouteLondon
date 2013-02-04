using BusRouteLondon.Migration;
using BusrRouteLondon.Web.Controllers;
using BusrRouteLondon.Web.Infrastructure.Indexes;
using NSubstitute;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Listeners;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace BusRouteLondon.Web.Tests.Controllers
{
    public abstract class RavenControllerTest<TController> : IDisposable where TController : RavenController, new()
    {
        private readonly EmbeddableDocumentStore documentStore;
        protected TController Controller { get; set; }

        protected RavenControllerTest()
        {
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8081);
            documentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true,
                    //UseEmbeddedHttpServer = true,
                };

            documentStore.RegisterListener(new NoStaleQueriesAllowed());
            documentStore.Initialize();
            IndexCreation.CreateIndexes(typeof(BusStop_Spatial).Assembly, documentStore);

            Controller = new TController { RavenSession = documentStore.OpenSession() };

            var httpContext = Substitute.For<HttpConfiguration>();
            var httpRoute = Substitute.For<HttpRouteData>(Substitute.For<IHttpRoute>());
            Controller.ControllerContext = new HttpControllerContext(httpContext, httpRoute, new HttpRequestMessage());
        }

        protected void SetupData(Action<IDocumentSession> action)
        {
            using (var session = documentStore.OpenSession())
            {
                action(session);
                session.SaveChanges();
            }
        }

        protected void SetupTestDB()
        {
            var filename = "Data/TestBusRoutes.csv";

            var fileParser = new BusRouteCSVParser();
            List<BusRoute> routes = fileParser.Parse(filename);

            var converter = new OSGB36ToWGS84();
            converter.ConvertRoutes(routes);

            SetupData(session => routes.ForEach(session.Store));
        }

        public class NoStaleQueriesAllowed : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }
        }

        public void Dispose()
        {
            documentStore.Dispose();
        }
    }
}