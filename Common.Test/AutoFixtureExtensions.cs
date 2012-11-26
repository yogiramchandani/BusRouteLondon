using NSubstitute;
using Ploeh.AutoFixture;

namespace Common.Test
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
