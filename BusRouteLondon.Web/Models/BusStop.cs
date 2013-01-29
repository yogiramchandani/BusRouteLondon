namespace BusRouteLondon.Web
{
    public class BusStop
    {
        public string Id;
        public string BusStopCode { get; set; }
        public string BusStopName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int Easting { get; set; }
        public int Northing { get; set; }
    }
}