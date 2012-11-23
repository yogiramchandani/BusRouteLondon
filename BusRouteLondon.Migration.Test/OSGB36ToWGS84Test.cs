
using System.Collections.Generic;
using Xunit;

namespace BusRouteLondon.Migration.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    public class OSGB36ToWGS84Test
    {
        [Fact]
        public void Convert_SetHolbornStationOSGB36_ExpectWGS84Latitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(new Dictionary<string, decimal> { { OSGB36ToWGS84.Easting, 530561 }, { OSGB36ToWGS84.Northing, 181433 } });
            Assert.Equal(51.516739639323m, result[OSGB36ToWGS84.Latitude]);
        }

        [Fact]
        public void Convert_SetHolbornStationOSGB36_ExpectWGS84Longitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(new Dictionary<string, decimal> { { OSGB36ToWGS84.Easting, 530561 }, { OSGB36ToWGS84.Northing, 181433 } });
            Assert.Equal(-0.11973122223921m, result[OSGB36ToWGS84.Longitude]);
        }

        [Fact]
        public void Convert_SetGlentrammonRoadOSGB36_ExpectWGS84Latitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(new Dictionary<string, decimal> { { OSGB36ToWGS84.Easting, 545562 }, { OSGB36ToWGS84.Northing, 163994 } });
            Assert.Equal(51.356377436224m, result[OSGB36ToWGS84.Latitude]);
        }

        [Fact]
        public void Convert_SetGlentrammonRoadOSGB36_ExpectWGS84Longitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(new Dictionary<string, decimal> { { OSGB36ToWGS84.Easting, 545562 }, { OSGB36ToWGS84.Northing, 163994 } });
            Assert.Equal(0.089153754011666m, result[OSGB36ToWGS84.Longitude]);
        }
    }
}
