namespace BusRouteLondon.Web
{
    public class BusStop
    {
        public string BusStopCode { get; set; }
        public string BusStopName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        
        public decimal Easting { get; set; }
        public decimal Northing { get; set; }
    }
}