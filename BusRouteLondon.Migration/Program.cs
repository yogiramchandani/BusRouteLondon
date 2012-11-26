using System;
using System.Linq;
using System.Net;
using Raven.Client.Document;

namespace BusRouteLondon.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started.");
            var fileParser = new BusRouteCSVParser();
            var filename = "BusRoute.csv";
            var routes = fileParser.Parse(filename);
            Console.WriteLine("Successfully read file {0}", filename);

            using (var documentStore = new DocumentStore { Url = "http://192.168.50.231:1717" }.Initialize())
            {
                Console.WriteLine("Successfully initialised RavenDB store.");

                // Store the company in our RavenDB server
                using (var session = documentStore.OpenSession("BusRouteLondonDB"))
                {
                    routes.ForEach(session.Store);
                    session.SaveChanges();
                }
            }
            Console.WriteLine("Successfully saved {0} routes.", routes.Count());
        }
    }
}
