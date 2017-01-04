using GameServer;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Hubs
{
	public class ServerHub : Hub
	{
        public void Start()
		{
			if (DedicatedGameServer.IsRunning)
			{
				//Already started
				return;
			}

            DedicatedGameServer.Start();
			Clients.All.updateStatus(DedicatedGameServer.GetStatus());
		}

		public void Stop()
		{
			if (!DedicatedGameServer.IsRunning)
			{
				//return "Not started";
				return;
			}

            DedicatedGameServer.Stop();
			Clients.All.updateStatus(DedicatedGameServer.GetStatus());
		}

        public void UpdateStatus()
        {
            Clients.Caller.updateStatus(DedicatedGameServer.GetStatus());
        }
    }
}
