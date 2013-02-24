
using BusRouteLondon.Web;
using BusrRouteLondon.Web.Migration;
using Raven.Client;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(BusrRouteLondon.Web.App_Start.TflDataMigration), "Initialise")]
namespace BusrRouteLondon.Web.App_Start
{
    public static class TflDataMigration
    {
        public static void Initialise()
        {
            try
            {
                var parser = DependencyResolver.Current.GetService<ICSVParser<BusRoute>>();
                var converter = DependencyResolver.Current.GetService<ISpatialCoordinateConverter>();
                var documentStore = DependencyResolver.Current.GetService<IDocumentStore>();

                using (var session = documentStore.OpenSession())
                {

                    var isDbEmpty = !session.Advanced.LuceneQuery<BusRoute>().WaitForNonStaleResultsAsOfLastWrite().Any();

                    if (isDbEmpty)
                    {
                        var filename = HttpContext.Current.Server.MapPath("~/App_Data/BusRoute.csv");
                        var routes = parser.Parse(filename);
                        converter.ConvertRoutes(routes);
                        routes.ForEach(session.Store);
                        session.SaveChanges();
                    }
                    isDbEmpty = !session.Advanced.LuceneQuery<BusRoute>().WaitForNonStaleResultsAsOfLastWrite().Any();
                    if (isDbEmpty)
                    {
                        
                    }
                }
            }
            catch (NullReferenceException)
            {
                HttpContext.Current.Response.Redirect("~/DBFailedToInitialise.htm");
            }
        }
    }
}