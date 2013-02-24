
using BusrRouteLondon.Web.Migration;
using Xunit;

namespace BusRouteLondon.Web.Tests
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
            var result = sut.Convert(530561, 181433);
            Assert.Equal(51.5167296657411, result.Lat, 13);
        }

        [Fact]
        public void Convert_SetHolbornStationOSGB36_ExpectWGS84Longitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(530561, 181433);
            Assert.Equal(-0.119746022882573d, result.Long, 13);
        }

        [Fact]
        public void Convert_SetGlentrammonRoadOSGB36_ExpectWGS84Latitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(545562, 163994);
            Assert.Equal(51.3563684129182d, result.Lat, 13);
        }

        [Fact]
        public void Convert_SetGlentrammonRoadOSGB36_ExpectWGS84Longitude()
        {
            var sut = new OSGB36ToWGS84();
            var result = sut.Convert(545562, 163994);
            Assert.Equal(0.0891389877307144d, result.Long, 13);
        }
    }
}
