using System;
using System.Collections.Generic;
using System.Linq;
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
            var proceed = true;
            do
            {
                Console.WriteLine("1. Press 1 to Migrate Bus Routes.");
                Console.WriteLine("2. Press 2 to update OSGB36 to WGS84.");
                Console.WriteLine();
                Console.Write("Press key: ");
                var input = Console.ReadKey();
                Console.WriteLine();
                proceed = true;
                switch (input.KeyChar)
                {
                    case '1':
                        MigrateBusRoute();
                        break;
                    case '2':
                        UpdateOSGB36ToWGS84();
                        break;
                    default:
                        Console.WriteLine("Incorrect key press: {0}, please try again.", input.KeyChar);
                        proceed = false;
                        break;
                }
            } while (!proceed);

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

            RavenDocumentStoreSessionOperation(session => routes.ForEach(session.Store));

            Console.WriteLine("Successfully saved {0} routes.", routes.Count);
        }

        private static void UpdateOSGB36ToWGS84()
        {
            Console.WriteLine("Updating OSGB36 to WGS84.");
            var converter = new OSGB36ToWGS84();
            RavenDocumentStoreSessionOperation(
                session =>
                    {
                        RavenQueryStatistics stats;
                        var page = -1;
                        var pageSize = 1000;
                        int skip;
                        do
                        {
                            page++;
                            skip = pageSize*page;
                            var routes =
                            session.Query<BusRoute>()
                            .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(50)))
                            .Statistics(out stats)
                            .OrderBy(x=>x.Route)
                            .Take(pageSize)
                            .Skip(skip)
                            .ToArray();

                            foreach (var busRoute in routes)
                            {
                                if (busRoute.Stop.Latitude != 0 && busRoute.Stop.Longitude != 0)
                                {
                                    continue;
                                }
                                var result = converter.Convert(new Dictionary<string, int> { { OSGB36ToWGS84.Easting, busRoute.Stop.Easting }, { OSGB36ToWGS84.Northing, busRoute.Stop.Northing } });
                                busRoute.Stop.Latitude = result[OSGB36ToWGS84.Latitude];
                                busRoute.Stop.Longitude = result[OSGB36ToWGS84.Longitude];
                            }   
                            session.SaveChanges();

                        } while (stats.TotalResults > skip);
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
