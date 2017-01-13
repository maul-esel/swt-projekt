using Xunit;

namespace Lingvo.Backend.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
			Assert.True(new Controllers.HomeController().DummyTestMethod()); // dummy test
        }
    }
}
