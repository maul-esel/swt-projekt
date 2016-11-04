using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Http;

namespace Calculator.Controllers
{
	[RoutePrefix("api/calculator")]
    public class CalculatorController : ApiController
    {
	    [Route("add/{n1}/{n2}"), HttpGet]
	    public int Add(int n1, int n2 )
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
