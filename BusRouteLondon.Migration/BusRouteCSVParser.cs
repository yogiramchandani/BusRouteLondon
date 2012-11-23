using BusRouteLondon.Web;

namespace BusRouteLondon.Migration
{
    public class BusRouteCSVParser : ICSVParser<BusRoute>
    {
        private ISpatialCoordinateConverter _converter;
        public BusRouteCSVParser(ISpatialCoordinateConverter converter)
        {
            _converter = converter;
        }

        public BusRoute[] Parse(string filename)
        {
            return new[] { new BusRoute { Stop = new BusStop { BusStopName = "" } } };
        }
    }

    public interface ICSVParser<T>
    {
        T[] Parse(string filename);
    }
}
