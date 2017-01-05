
using System;
using GameServer;


namespace ServerExtender
{
	class Program
	{
		const string CONTENT_PATH = @"E:\Program Files (x86)\Steam\SteamApps\common\SpaceEngineers";


		[STAThread]
		static void Main(string[] args)
		{
			Console.SetOut(ConsoleHandler.Instance);
			Console.WriteLine("Initializing...");

			WebServer webServer = new WebServer();
			webServer.Start();

			Console.WriteLine("Server started.");


			Console.WriteLine("Type 'quit' to exit the server.");
			while (ConsoleHandler.Instance.StayOpen)
			{
				string command = Console.ReadLine();
				ConsoleHandler.Instance.ExecuteCommand("[CONSOLE]", command);
			}

			Console.WriteLine("Cleaning Game server...");
			DedicatedGameServer.Clean();

			Console.WriteLine("Closing the webserver...");
			webServer.Stop();
		}

	}
}
