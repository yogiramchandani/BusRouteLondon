using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusRouteLondon.Web;
using Microsoft.VisualBasic.FileIO;

namespace BusRouteLondon.Migration
{
    public class BusRouteCSVParser : ICSVParser<BusRoute>
    {
        public List<BusRoute> Parse(string filename)
        {
            var routes = new List<BusRoute>();
            using (var parser = new TextFieldParser(filename))
            {
                parser.Delimiters = new string[] { "," };
                parser.HasFieldsEnclosedInQuotes = true;
                
                parser.ReadLine().Skip(1);

                while (true)
                {
                    string[] x = parser.ReadFields();
                    if (x == null || x.Count() < 9)
                    {
                        return routes;
                    }
                    int heading;
                    string routeNum = x[(int) BusCSVFields.Route];
                    int runNumber = int.Parse(x[(int) BusCSVFields.Run]);
                    int sequenceNumber = int.Parse(x[(int) BusCSVFields.Sequence]);
                    var route = new BusRoute
                                    {   
                                        Route = routeNum,
                                        Run = runNumber,
                                        Sequence = sequenceNumber,
                                        Heading = int.TryParse(x[(int)BusCSVFields.Heading], out heading) ? heading : 0
                                    };
                    route.Stop = GetStop(x);
                    routes.Add(route);
                }
            }
        }

        private Dictionary<string, BusStop> _stopDictionary = new Dictionary<string, BusStop>();
        private BusStop GetStop(string[] parsedCsvStrings)
        {
            string busStopCode = HttpUtility.HtmlEncode(parsedCsvStrings[(int) BusCSVFields.BusStopCode]);
            if (!_stopDictionary.ContainsKey(busStopCode))
            {
                var newStop = new BusStop
                                  {
                                      Id = string.Format("busstops/{0}",busStopCode),
                                      BusStopCode = busStopCode,
                                      BusStopName = HttpUtility.HtmlEncode(parsedCsvStrings[(int) BusCSVFields.BusStopName]),
                                      Easting = int.Parse(parsedCsvStrings[(int) BusCSVFields.Easting]),
                                      Northing = int.Parse(parsedCsvStrings[(int) BusCSVFields.Northing])
                                  };
                _stopDictionary.Add(busStopCode, newStop);
            }

            return _stopDictionary[busStopCode];
        }
    }

    internal enum BusCSVFields
    {
        Route = 0,
        Run = 1,
        Sequence = 2,
        BusStopCode = 4,
        BusStopName = 6,
        Easting = 7,
        Northing = 8,
        Heading = 9
    }

    public interface ICSVParser<T>
    {
        List<T> Parse(string filename);
    }
}
