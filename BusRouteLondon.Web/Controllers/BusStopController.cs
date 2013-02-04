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
        public BusStopWithCount Get(double lat, double lng, double radius)
        {
            RavenQueryStatistics stats;
            IDocumentQuery<BusStop> stops = RavenSession.Advanced.LuceneQuery<BusStop, BusStop_Spatial>().WithinRadiusOf(radius: radius, latitude: lat, longitude: lng).Statistics(out stats);
            
            return new BusStopWithCount{Stops = stops.ToList(), TotalCount = stats.TotalResults};
        }

        public class BusStopWithCount
        {
            public List<BusStop> Stops { get; set; }
            public int TotalCount { get; set; }
        }
    }
}