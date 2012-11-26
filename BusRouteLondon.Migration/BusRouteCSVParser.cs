using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
                    var route = new BusRoute
                                    {
                                        Route = x[0],
                                        Run = int.Parse(x[1]),
                                        Sequence = int.Parse(x[2]),
                                        Heading = int.TryParse(x[9], out heading) ? heading : 0,
                                        Stop = new BusStop
                                                   {
                                                       BusStopCode = HttpUtility.HtmlEncode(x[4]),
                                                       BusStopName = HttpUtility.HtmlEncode(x[6]),
                                                       Easting = decimal.Parse(x[7]),
                                                       Northing = decimal.Parse(x[8])
                                                   }
                                    };
                    routes.Add(route);
                }
            }
        }
    }

    public interface ICSVParser<T>
    {
        List<T> Parse(string filename);
    }
}
