using System;
using System.Linq;
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

            using (var documentStore = new DocumentStore { ConnectionStringName = "RavenDB" })
            {
                documentStore.Initialize();
                Console.WriteLine("Successfully initialised RavenDB store.");

                // Store the company in our RavenDB server
                using (var session = documentStore.OpenSession("BusRouteLondonDB"))
                {
                    routes.ForEach(session.Store);
                    session.SaveChanges();
                }
            }

            Console.WriteLine("Successfully saved {0} routes.", routes.Count());
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
