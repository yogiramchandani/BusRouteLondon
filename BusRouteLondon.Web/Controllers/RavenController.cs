using Raven.Client;
using System.Web.Http;

namespace BusRouteLondon.Web.Controllers
{
    public abstract class RavenController : ApiController
    {
        public IDocumentSession RavenSession { get; set; }
    }
}
