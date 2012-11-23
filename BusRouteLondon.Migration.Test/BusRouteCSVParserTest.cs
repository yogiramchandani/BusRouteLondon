using BusRouteLondon.Web;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace BusRouteLondon.Migration
{
    public class BusRouteCSVParserTest
    {
        [Theory, AutoData]
        public void Parse_FirstLine_ExpectNewOxfordStreetBusStopName(OSGB36ToWGS84 converter)
        {
            BusRoute expected = new BusRoute { Stop = new BusStop { BusStopName = "NEW OXFORD STREET" } };

            var filename = "testfile.csv";
            var sut = new BusRouteCSVParser(converter);
            var actual = sut.Parse(filename);
            Assert.Equal(expected.Stop.BusStopName, actual[0].Stop.BusStopName);
        }
    }
}
