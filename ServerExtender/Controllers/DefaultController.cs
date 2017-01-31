using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerExtender.Controllers
{
	public class DefaultController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Index()
		{
			string html = "";
			using (StreamReader reader = new StreamReader("www/index.html"))
			{
				html = reader.ReadToEnd();
			}

			var response = new HttpResponseMessage();
			response.Content = new StringContent(html);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

			return response;
		}

	}
}
