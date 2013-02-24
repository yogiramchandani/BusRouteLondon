using NSubstitute;
using Ploeh.AutoFixture;

namespace BusRouteLondon.Web.Tests
{
    public static class AutoFixtureExtensions
    {
        public static T FreezeNSubstitute<T>(this Fixture fixture) where T : class
        {
            T mock = Substitute.For<T>();
            fixture.Register(() => mock);
            return mock;
        }
    }
}
