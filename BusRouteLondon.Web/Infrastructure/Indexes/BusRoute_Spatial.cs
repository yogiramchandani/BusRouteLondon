using System.Linq;
using BusRouteLondon.Web.Models;
using Raven.Client.Indexes;

namespace BusRouteLondon.Web.Infrastructure.Indexes
{
    public class BusRoute_Spatial : AbstractIndexCreationTask<BusRoute>
    {
        public BusRoute_Spatial()
        {
            Map = busRoutes => from busRoute in busRoutes
                               select new
                                   {
                                       _ = SpatialIndex.Generate(busRoute.Stop.Latitude, busRoute.Stop.Longitude)
                                   };
        }
    }
}