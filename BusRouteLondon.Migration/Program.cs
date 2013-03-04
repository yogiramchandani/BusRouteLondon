using System;
using System.Collections.Generic;
using BusRouteLondon.Web;
using BusRouteLondon.Web.Migration;
using BusRouteLondon.Web.Models;
using Raven.Client;
using Raven.Client.Document;

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
            var filename = "BusRoute.csv";
            List<BusRoute> routes = ConsoleParser.For(new BusRouteCSVParser()).Parse(filename);
            
            var converter = new OSGB36ToWGS84();
            converter.ConvertRoutes(routes);

            RavenDocumentStoreSessionOperation(session => routes.ForEach(session.Store));

            Console.WriteLine("Successfully saved {0} routes.", routes.Count);
        }

        private static void RavenDocumentStoreSessionOperation(Action<IDocumentSession> action)
        {
            using (var documentStore = new DocumentStore {ConnectionStringName = "RavenDB", DefaultDatabase = "BusRouteLondonDB"})
            {
                documentStore.Initialize();
                
                
                using (var session = documentStore.OpenSession())
                {
                    action(session);
                    session.SaveChanges();
                }
            }
        }
    }
}
