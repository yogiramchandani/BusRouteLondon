using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusRouteLondon.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace BusRouteLondon.Migration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Started.");

            MigrateBusRoute();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void MigrateBusRoute()
        {
            Console.WriteLine("Read File and parse bus routes.");

            var filename = "BusRoute.csv";
            var fileParser = new BusRouteCSVParser();
            List<BusRoute> routes = fileParser.Parse(filename);

            Console.WriteLine("Successfully read file {0}", filename);
            Console.WriteLine("Updating OSGB36 to WGS84.");
            
            var converter = new OSGB36ToWGS84();
            converter.ConvertRoutes(routes);

            RavenDocumentStoreSessionOperation(session => routes.ForEach(session.Store));

            Console.WriteLine("Successfully saved {0} routes.", routes.Count);
        }
        
        private static void RavenDocumentStoreSessionOperation(Action<IDocumentSession> action)
        {
            const string BusDB = "BusRouteLondonDB";
            using (var documentStore = new DocumentStore {ConnectionStringName = "RavenDB"})
            {
                documentStore.Initialize();
                
                documentStore.DatabaseCommands.EnsureDatabaseExists(BusDB);
                documentStore.Conventions.RegisterIdConvention<BusRoute>(
                    (dbName, command, route) => string.Format("busroutes/{0}-{1}-{2}", route.Route, route.Run, route.Sequence));
                using (var session = documentStore.OpenSession(BusDB))
                {
                    action(session);
                    session.SaveChanges();
                }
            }
        }
    }
}
