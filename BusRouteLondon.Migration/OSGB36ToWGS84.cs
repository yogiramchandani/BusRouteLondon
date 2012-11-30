using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BusRouteLondon.Migration
{
    public class OSGB36ToWGS84 : ISpatialCoordinateConverter
    {
        private Dictionary<string, Dictionary<string, double>> Cached;

        public const string Latitude = "lat";
        public const string Longitude = "lng";

        public const string Easting = "easting";
        public const string Northing = "northing";

        public OSGB36ToWGS84()
        {
            Cached = new Dictionary<string, Dictionary<string, double>>();
        }

        public Dictionary<string, double> Convert(Dictionary<string, int> input)
        {
            string key = string.Format("{0}:{1}", input[Easting], input[Northing]);
            
            if (!Cached.ContainsKey(key))
            {
                Cached.Add(key, CallWebService(input));
            }
            return Cached[key];
        }

        private Dictionary<string, double> CallWebService(Dictionary<string, int> input)
        {
            string url = string.Format("http://www.uk-postcodes.com/eastingnorthing.php?easting={0}&northing={1}", input[Easting], input[Northing]);

            var request = System.Net.HttpWebRequest.Create(url);
            
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var webResult = reader.ReadToEnd();
                    var jsonObject = JObject.Parse(webResult);
                    var latitudeString = (string)jsonObject[Latitude];
                    var longitudeString = (string)jsonObject[Longitude];

                    return new Dictionary<string, double> { { Latitude, double.Parse(latitudeString) }, { Longitude, double.Parse(longitudeString) } };
                }    
            }
        }
    }

    public interface ISpatialCoordinateConverter
    {
        Dictionary<string, double> Convert(Dictionary<string, int> input);
    }
}