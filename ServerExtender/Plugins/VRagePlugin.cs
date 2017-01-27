using ServerExtender.Plugins.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtender.Plugins
{
	public class VRagePlugin : VRage.Plugins.IPlugin
	{
		public EventHandler<VRagePluginInitArgs> PluginInit;
		public EventHandler PluginUpdate;
		public EventHandler PluginDispose;

		public void Init(object gameInstance)
		{
			PluginInit?.Invoke(this, new VRagePluginInitArgs(gameInstance));
		}

		public void Update()
		{
			PluginUpdate?.Invoke(this, EventArgs.Empty);
		}

		public void Dispose()
		{
			PluginDispose?.Invoke(this, EventArgs.Empty);
		}
	}
}
