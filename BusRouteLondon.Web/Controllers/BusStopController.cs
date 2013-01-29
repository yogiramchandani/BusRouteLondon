using System;
using System.Collections.Generic;
using System.Linq;
using BusrRouteLondon.Web.Controllers;
using BusrRouteLondon.Web.Infrastructure.Indexes;

namespace BusRouteLondon.Web.Controllers
{
    public class BusStopController : RavenController
    {
        // GET api/values/5
        public IEnumerable<BusStop> Get(double lat, double lng, double radius)
        {
            return
                RavenSession.Advanced.LuceneQuery<BusStop>("BusStop/BusStopCode")
                    .WithinRadiusOf(radius: radius, latitude: lat, longitude: lng);
        }
    }
}