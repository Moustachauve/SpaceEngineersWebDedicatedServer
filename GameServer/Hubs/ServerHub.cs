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
            DedicatedGameServer.Start();
		}

		public void Stop()
		{
            DedicatedGameServer.Stop();
		}

        public void UpdateStatus()
        {
            Clients.Caller.updateStatus(DedicatedGameServer.Status.ToString());
        }
    }
}
