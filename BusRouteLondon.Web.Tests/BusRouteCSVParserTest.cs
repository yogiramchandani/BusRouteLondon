using System.Collections.Generic;
using System.Linq;
using BusrRouteLondon.Web.Migration;
using Xunit;

namespace BusRouteLondon.Web.Tests
{
    public class BusRouteCSVParserTest
    {
        [Fact]
        public void Parse_FirstLine_ExpectRoute()
        {
            Assert.Equal("1", Routes.ElementAt(0).Route);
        }

        [Fact]
        public void Parse_FirstLine_ExpectRun()
        {
            Assert.Equal(1, Routes.ElementAt(0).Run);
        }

        [Fact]
        public void Parse_FirstLine_ExpectHeading()
        {
            Assert.Equal(75, Routes.ElementAt(0).Heading);
        }

        [Fact]
        public void Parse_FirstLine_ExpectStopCode()
        {
            Assert.Equal("53369", Routes.ElementAt(0).Stop.BusStopCode);
        }

        [Fact]
        public void Parse_FirstLine_ExpectStopName()
        {
            Assert.Equal("NEW OXFORD STREET", Routes.ElementAt(0).Stop.BusStopName);
        }

        [Fact]
        public void Parse_SecondLine_ExpectSequence()
        {
            Assert.Equal(2, Routes.ElementAt(1).Sequence);
        }

        [Fact]
        public void Parse_FirstLine_ExpectEasting()
        {
            Assert.Equal(529956m, Routes.ElementAt(0).Stop.Easting);
        }

        [Fact]
        public void Parse_FirstLine_ExpectNorthing()
        {
            Assert.Equal(181417m, Routes.ElementAt(0).Stop.Northing);
        }

        private IEnumerable<BusRoute> _routes;
        private IEnumerable<BusRoute> Routes
        {
            get
            {
                if (_routes == null)
                {
                    var sut = new BusRouteCSVParser();
                    const string filename = "Data/TestBusRoutes.csv";
                    _routes = sut.Parse(filename);
                }
                return _routes;
            }
        }
    }
}
