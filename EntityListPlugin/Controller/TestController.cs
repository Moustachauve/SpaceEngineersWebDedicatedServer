using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerExtender.Controller
{
	public class TestController : ApiController
	{
		[HttpGet]
		public IHttpActionResult Index()
		{
			return Ok("Hello World!");
		}

	}
}
