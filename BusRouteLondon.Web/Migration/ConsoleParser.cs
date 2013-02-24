using System;
using System.Collections.Generic;

namespace BusrRouteLondon.Web.Migration
{
    // Console logging decorator
    public sealed class ConsoleParser<T> : ICSVParser<T>
    {
        private readonly ICSVParser<T> _parser;

        public ConsoleParser(ICSVParser<T> parser)
        {
            _parser = parser;
        }

        public List<T> Parse(string filename)
        {
            Console.WriteLine("Read File and parse bus routes.");
            var result = _parser.Parse(filename);
            Console.WriteLine("Successfully read file {0}", filename);
            Console.WriteLine("Updating OSGB36 to WGS84.");
            return result;
        }
    }

    public static class ConsoleParser
    {
        public static ICSVParser<T> For<T>(ICSVParser<T> parser)
        {
            return new ConsoleParser<T>(parser);
        }
    }
}