using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Raven.Client;

namespace BusrRouteLondon.Web.Controllers
{
    public abstract class RavenController : ApiController
    {
        public static string CurrentRequestRavenSession = "CurrentRequestRavenSession";

        public static IDocumentStore DocumentStore { get; set; }

        public IDocumentSession RavenSession { get; set; }

        protected override void Initialize(HttpControllerContext filterContext)
        {
            RavenSession = (IDocumentSession)HttpContext.Current.Items[CurrentRequestRavenSession];
        }
    }
}
