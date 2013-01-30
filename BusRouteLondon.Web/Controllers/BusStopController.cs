using System;
using System.Collections.Generic;
using System.Linq;
using BusrRouteLondon.Web.Controllers;
using BusrRouteLondon.Web.Infrastructure.Indexes;
using Raven.Client;

namespace BusRouteLondon.Web.Controllers
{
    public class BusStopController : RavenController
    {
        public IEnumerable<BusStop> Get(double lat, double lng, double radius)
        {
            IDocumentQuery<BusStop> stops = RavenSession.Advanced.LuceneQuery<BusStop, BusStop_Spatial>().WithinRadiusOf(radius: radius, latitude: lat, longitude: lng);
            return stops;
        }
    }
}