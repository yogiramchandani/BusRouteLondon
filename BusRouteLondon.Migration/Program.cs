using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusRouteLondon.Web;
using Raven.Client;
using Raven.Client.Document;

namespace BusRouteLondon.Migration
{
    class Program
    {
        static void Main(string[] args)
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
                                                     converter.Convert(new Dictionary<string, int>
                                                                           {
                                                                               {OSGB36ToWGS84.Easting, route.Stop.Easting},
                                                                               {OSGB36ToWGS84.Northing, route.Stop.Northing}
                                                                           });
                                             if (result != null)
                                             {
                                                 route.Stop.Latitude = result[OSGB36ToWGS84.Latitude];
                                                 route.Stop.Longitude = result[OSGB36ToWGS84.Longitude];
                                             }
                                             Console.WriteLine("Updating OSGB36 to WGS84 for stop {0}.", route.Stop.Id);
                                         });
        }

        private static void RavenDocumentStoreSessionOperation(Action<IDocumentSession> action )
        {
            using (var documentStore = new DocumentStore { ConnectionStringName = "RavenDB" })
            {
                documentStore.Initialize();
                Console.WriteLine("Successfully initialised RavenDB store.");

                using (var session = documentStore.OpenSession("BusRouteLondonDB"))
                {
                    action(session);
                    session.SaveChanges();
                }
            }
        }
    }
}
