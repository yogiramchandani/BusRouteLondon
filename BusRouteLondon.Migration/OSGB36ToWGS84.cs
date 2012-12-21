using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace BusRouteLondon.Migration
{
    public class OSGB36ToWGS84 : ISpatialCoordinateConverter
    {
        private ConcurrentDictionary<string, Dictionary<string, double>> Cached;
        private ConcurrentDictionary<string, string> Exceptions;
        
        public const string Latitude = "lat";
        public const string Longitude = "lng";

        public const string Easting = "easting";
        public const string Northing = "northing";

        public OSGB36ToWGS84()
        {
            Cached = new ConcurrentDictionary<string, Dictionary<string, double>>();
            Exceptions = new ConcurrentDictionary<string, string>();
        }

        public Dictionary<string, double> Convert(Dictionary<string, int> input)
        {
            string key = string.Format("{0}:{1}", input[Easting], input[Northing]);

            if (!Cached.ContainsKey(key))
            {
                Cached.TryAdd(key, CallWebService(input));
            }
            else
            {
                Console.WriteLine("Cache used");
            }
            return Cached[key];
        }

        private Dictionary<string, double> CallWebService(Dictionary<string, int> input)
        {
            try
            {
                string url = string.Format("http://www.uk-postcodes.com/eastingnorthing.php?easting={0}&northing={1}", input[Easting], input[Northing]);

                var request = System.Net.HttpWebRequest.Create(url);

                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var webResult = reader.ReadToEnd();
                        var jsonObject = JObject.Parse(webResult);
                        var latitudeString = (string) jsonObject[Latitude];
                        var longitudeString = (string) jsonObject[Longitude];

                        return new Dictionary<string, double>
                                   {
                                       {Latitude, double.Parse(latitudeString)},
                                       {Longitude, double.Parse(longitudeString)}
                                   };
                    }
                }
            }
            catch (WebException)
            {
                string key = string.Format("{0}:{1}", input[Easting], input[Northing]);
                if (!Exceptions.ContainsKey(key))
                {
	                Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("1st exception, retry for route {0}", key);
                    Console.ResetColor();
                    Exceptions.TryAdd(key, key);
                    return CallWebService(input);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("2nd exception, cancelling request for route {0}", key);
                    Console.ResetColor();
                    return null;
                }
            }
        }
    }

    public interface ISpatialCoordinateConverter
    {
        Dictionary<string, double> Convert(Dictionary<string, int> input);
    }
}