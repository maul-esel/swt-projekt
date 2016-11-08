using Microsoft.AspNetCore.Mvc;

namespace Calculator.Controllers
{
    [Route("api/calculator")]
    public class CalculatorController : Controller
    {
		[Route("add/{n1}/{n2}"), HttpGet]
		public int Add(int n1, int n2)
		{
			return n1 + n2;
		}

		[Route("multiply/{n1}/{n2}"), HttpGet]
		public int Multiply(int n1, int n2)
		{
			return n1 * n2;
		}
	}
}
