using GameServer;
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
			return DedicatedGameServer.GetStatus();
		}

		[HttpGet]
		public string Start()
		{
			if (DedicatedGameServer.IsRunning)
			{
				return "Already started";
			}

            DedicatedGameServer.Start();
			return "Ok";
		}

		[HttpGet]
		public string Stop()
		{
			if (!DedicatedGameServer.IsRunning)
			{
				return "Not started";
			}

            DedicatedGameServer.Stop();
			return "Ok";
		}
	}
}
