using GameServer;
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

        public ServerHub()
        {
            //TODO: Do not use an event here, this is created more than once!!
            DedicatedGameServer.ServerEvents.StatusChanged += OnServerStatusChanged;
        }

        private void OnServerStatusChanged(object sender, EventArgs args)
        {
            Clients.All.updateStatus(DedicatedGameServer.GetStatus());
        }

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

        public void GetConsoleText()
        {
            Clients.Caller.consoleReplace(ConsoleHandler.Instance.Log);
        }
    }
}
