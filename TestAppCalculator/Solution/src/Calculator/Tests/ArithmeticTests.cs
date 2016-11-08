using Xunit;
using Calculator.Controllers;

namespace Calculator.Test
{
    public class ArithmeticTests
    {
		[Fact]
		public void TestOnePlusOne()
		{
			var controller = new CalculatorController();
			Assert.Equal(2, controller.Add(1, 1));
		}
    }
}
