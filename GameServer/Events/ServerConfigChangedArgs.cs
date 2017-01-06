using Sandbox.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;

namespace GameServer.Events
{
	public class ServerConfigChangedArgs
	{
		public MyConfigDedicated<MyObjectBuilder_SessionSettings> ServerConfig { get; private set; }

		public ServerConfigChangedArgs(MyConfigDedicated<MyObjectBuilder_SessionSettings> serverConfig)
		{
			ServerConfig = serverConfig;
		}
	}
}
