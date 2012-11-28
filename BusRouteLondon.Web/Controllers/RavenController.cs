using System.Web;
using System.Web.Mvc;
using Raven.Client;

namespace BusrRouteLondon.Web.Controllers
{
    public abstract class RavenController : Controller
    {
        public static string CurrentRequestRavenSession = "CurrentRequestRavenSession";

        public static IDocumentStore DocumentStore { get; set; }

        public IDocumentSession RavenSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = (IDocumentSession)HttpContext.Items[CurrentRequestRavenSession];
        }
    }
}
