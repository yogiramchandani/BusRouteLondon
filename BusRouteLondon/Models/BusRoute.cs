namespace BusRouteLondon.Web
{
    public class BusRoute
    {
        public int Route { get; set; }
        public int Run { get; set; }
        public int Sequence { get; set; }
        public BusStop Stop { get; set; }
        public int Heading { get; set; }
    }
}