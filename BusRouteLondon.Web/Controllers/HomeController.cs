using System.Web.Mvc;
using BusrRouteLondon.Web.Migration;
using Ninject;

namespace BusRouteLondon.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
