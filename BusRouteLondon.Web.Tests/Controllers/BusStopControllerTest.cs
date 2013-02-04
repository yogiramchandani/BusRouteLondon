using System.Collections.Generic;
using System.Linq;
using BusRouteLondon.Migration;
using BusRouteLondon.Web.Controllers;
using Xunit;

namespace BusRouteLondon.Web.Tests.Controllers
{
    public class BusStopControllerTest : RavenControllerTest<BusStopController>
    {
        public BusStopControllerTest()
        {
            SetupTestDB();
        }

        [Fact]
        public void Get_ForOxfordCircus_Expect9Stops()
        {
            // Arrange
            var lat = 51.51499804271553;
            var lng = -0.1409999999999627;
            var radius = 1.6757531522774072;
            
            // Act
            var result = Controller.Get(lat: lat, lng: lng, radius: radius);
            result.Stops.ToList();

            // Assert
            Assert.Equal(9, result.Stops.Count());
            Assert.Equal(9, result.TotalCount);
        }
    }
}
