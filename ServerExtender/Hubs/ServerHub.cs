using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtender.Hubs
{
	public class ServerHub : Hub
	{
		public void Start()
		{
			if (GameServer.IsRunning)
			{
				//Already started
				return;
			}


			GameServer.Start();
			Clients.All.updateStatus(GameServer.GetStatus());
		}

		public void Stop()
		{
			if (!GameServer.IsRunning)
			{
				//return "Not started";
				return;
			}

			GameServer.Stop();
			Clients.All.updateStatus(GameServer.GetStatus());
		}

		public void UpdateStatus()
		{
			Clients.Caller.updateStatus(GameServer.GetStatus());
		}
	}
}
