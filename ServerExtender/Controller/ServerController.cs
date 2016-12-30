using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerExtender.Controller
{
	public class ServerController : ApiController
	{
		[HttpGet]
		public string Status()
		{
			if (GameServer.IsReady)
			{
				return "Started";
			}
			else if(GameServer.IsRunning)
			{
				return "Starting";
			}
			else
			{
				return "Stopped";
			}
		}

		[HttpGet]
		public string Start()
		{
			if(GameServer.IsRunning)
			{
				return "Already started";
			}

			GameServer.Start();
			return "Ok";
		}

		[HttpGet]
		public string Stop()
		{
			if (!GameServer.IsRunning)
			{
				return "Not started";
			}

			GameServer.Stop();
			return "Ok";
		}
	}
}
