using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BusRouteLondon.Migration
{
    public class OSGB36ToWGS84 : ISpatialCoordinateConverter
    {
        public const string Latitude = "lat";
        public const string Longitude = "lng";

        public const string Easting = "easting";
        public const string Northing = "northing";

        public Dictionary<string, decimal> Convert(Dictionary<string, decimal> input)
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

                    return new Dictionary<string, decimal> { { Latitude, decimal.Parse(latitudeString) }, { Longitude, decimal.Parse(longitudeString) } };
                }    
            }
        }
    }

    public interface ISpatialCoordinateConverter
    {
        Dictionary<string, decimal> Convert(Dictionary<string, decimal> input);
    }
}