using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Events
{
	public class ServerStatusChangedArgs
	{
		public ServerStatus OldServerStatus { get; private set; }
		public ServerStatus NewServerStatus { get; private set; }

		public ServerStatusChangedArgs(ServerStatus oldServerStatus, ServerStatus newServerStatus)
		{
			OldServerStatus = oldServerStatus;
			NewServerStatus = newServerStatus;
		}
	}
}
