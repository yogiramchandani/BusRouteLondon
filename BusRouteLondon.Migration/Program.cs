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
            Console.WriteLine("Migrating Bus Routes.");
            var fileParser = new BusRouteCSVParser();
            var filename = "BusRoute.csv";
            List<BusRoute> routes = fileParser.Parse(filename);
            Console.WriteLine("Successfully read file {0}", filename);
            UpdateOSGB36ToWGS84(routes);

            RavenDocumentStoreSessionOperation(session => routes.ForEach(session.Store));

            Console.WriteLine("Successfully saved {0} routes.", routes.Count);
        }

        private static void UpdateOSGB36ToWGS84(List<BusRoute> routes)
        {
            Console.WriteLine("Updating OSGB36 to WGS84.");
            var converter = new OSGB36ToWGS84();

            Parallel.ForEach(routes, route =>
            {
                if (route.Stop.Longitude != 0 && route.Stop.Latitude != 0)
                {
                    Console.WriteLine("Skipped ! for {0}", route.Id);
                    return;
                }
                var result =
                    converter.Convert(route.Stop.Easting, route.Stop.Northing);
                
                route.Stop.Latitude = result.Lat;
                route.Stop.Longitude = result.Long;
                Console.WriteLine("Updating OSGB36 to WGS84 for stop {0}.", route.Stop.Id);
            });
        }

        private static void RavenDocumentStoreSessionOperation(Action<IDocumentSession> action)
        {
            const string BusDB = "BusRouteLondonDB";
            using (var documentStore = new DocumentStore {ConnectionStringName = "RavenDB"})
            {
                documentStore.Initialize();
                Console.WriteLine("Successfully initialised RavenDB store.");

                documentStore.DatabaseCommands.EnsureDatabaseExists(BusDB);
                documentStore.Conventions.RegisterIdConvention<BusRoute>(
                    (dbName, command, route) =>
                    string.Format("busroutes/{0}-{1}-{2}", route.Route, route.Run, route.Sequence));
                using (var session = documentStore.OpenSession(BusDB))
                {
                    action(session);
                    session.SaveChanges();
                }
            }
        }
    }
}
