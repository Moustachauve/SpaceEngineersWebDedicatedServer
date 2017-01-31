namespace ServerPlugin.Events
{
	public class VRagePluginInitArgs
	{
		public object GameInstance { get; private set; }

		public VRagePluginInitArgs(object gameInstance)
		{
			GameInstance = gameInstance;
		}
	}
}