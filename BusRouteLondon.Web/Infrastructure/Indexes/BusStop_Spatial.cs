using System.Linq;
using BusRouteLondon.Web;
using Raven.Client.Indexes;

namespace BusrRouteLondon.Web.Infrastructure.Indexes
{
    public class BusStop_Spatial : AbstractIndexCreationTask<BusRoute, BusStop>
    {
        public BusStop_Spatial()
        {
            Map = busRoutes => from busRoute in busRoutes
                               select new
                                          {
                                              _ = (object)null,
                                              busRoute.Stop.Id,
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
                                                          _ = (object) null,
                                                          busStop.Id,
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
                                                _ = SpatialIndex.Generate(g.Key.Latitude, g.Key.Longitude),
                                                g.Key.Id,
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