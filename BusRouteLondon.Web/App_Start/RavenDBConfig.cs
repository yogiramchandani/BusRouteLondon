using BusRouteLondon.Web.Models;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using System.Configuration;

namespace BusRouteLondon.Web.App_Start
{
    public class RavenDbConfig
    {
        public IDocumentStore GetRavenDBStore()
        {
            var hasRavenConnectionString = ConfigurationManager.ConnectionStrings["RavenDB"] != null;
            IDocumentStore docStore;
            if (hasRavenConnectionString)
            {
                docStore = new DocumentStore
                    {
                        ConnectionStringName = "RavenDB",
                        DefaultDatabase = "BusRouteLondonDB"
                    };
            }
            else
            {
                //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
                docStore = new EmbeddableDocumentStore
                    {
                        //RunInMemory = true,
                        DataDirectory = "~/App_Data/Raven",
                        //UseEmbeddedHttpServer = true
                    };
            }

            docStore.Initialize();

            docStore.Conventions.RegisterIdConvention<BusRoute>(
                (dbName, command, route) =>
                string.Format("busroutes/{0}-{1}-{2}", route.Route, route.Run, route.Sequence));

            return docStore;
        }
    }
}