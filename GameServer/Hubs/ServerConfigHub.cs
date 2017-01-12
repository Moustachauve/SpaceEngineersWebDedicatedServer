using GameServer.Helpers;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Hubs
{
	public class ServerConfigHub : Hub
	{
		public void GetConfig()
		{
			Clients.Caller.replaceConfig(DedicatedGameServer.ServerConfig);
		}

		public void SetValue(string key, object value)
		{
			if (DedicatedGameServer.Status != ServerStatus.Stopped)
				return;

			object newValue = ReflexionHelper.SetValueFromKey(DedicatedGameServer.ServerConfig, key, value);
			DedicatedGameServer.SaveServerConfig();

			Clients.All.setValue(key, newValue);
		}

		public void ReloadConfig()
		{
			Clients.Caller.replaceConfig(/*DedicatedGameServer.ReloadServerConfig()*/);
		}
	}
}
