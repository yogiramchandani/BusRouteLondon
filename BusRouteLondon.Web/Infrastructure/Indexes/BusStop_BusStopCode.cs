using System.Linq;
using BusRouteLondon.Web;
using Raven.Client.Indexes;

namespace BusrRouteLondon.Web.Infrastructure.Indexes
{
    public class BusStop_BusStopCode : AbstractIndexCreationTask<BusRoute, BusStop>
    {
        public BusStop_BusStopCode()
        {
            Map = busRoutes => from busRoute in busRoutes
                               select new
                                          {
                                              busRoute.Stop.BusStopCode,
                                              busRoute.Stop.BusStopName,
                                              busRoute.Stop.Latitude,
                                              busRoute.Stop.Longitude,
                                              busRoute.Stop.Easting,
                                              busRoute.Stop.Northing
                                          };
            Reduce = busStops => from busStop in busStops
                                 group busStop by new
                                                      {
                                                          busStop.BusStopCode,
                                                          busStop.BusStopName,
                                                          busStop.Latitude,
                                                          busStop.Longitude,
                                                          busStop.Easting,
                                                          busStop.Northing
                                                      }
                                 into g
                                 select new
                                            {
                                                g.Key.BusStopCode,
                                                g.Key.BusStopName,
                                                g.Key.Latitude,
                                                g.Key.Longitude,
                                                g.Key.Easting,
                                                g.Key.Northing
                                            };
        }
    }
}