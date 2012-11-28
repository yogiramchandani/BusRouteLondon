using System.Linq;
using BusRouteLondon.Web.Controllers;
using Xunit;

namespace BusRouteLondon.Web.Tests.Controllers
{
    public class ValuesControllerTest
    {
        [Fact]
        public void Get()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("value1", result.ElementAt(0));
            Assert.Equal("value2", result.ElementAt(1));
        }

        [Fact]
        public void GetById()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            string result = controller.Get(5);

            // Assert
            Assert.Equal("value", result);
        }

        [Fact]
        public void Post()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Post("value");

            // Assert
        }

        [Fact]
        public void Put()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Put(5, "value");

            // Assert
        }

        [Fact]
        public void Delete()
        {
            // Arrange
            ValuesController controller = new ValuesController();

            // Act
            controller.Delete(5);

            // Assert
        }
    }
}
