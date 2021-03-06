﻿using System;
using GameServer;
using ServerPlugin;

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

			Console.WriteLine("Web server started.");
			Console.WriteLine("Loading plugins...");

			PluginManager.Instance.ScanPlugins();
			PluginManager.Instance.LoadPlugins();

			Console.WriteLine("Plugins loaded.");

			Console.WriteLine("Type 'quit' to exit the server.");
			while (ConsoleHandler.Instance.StayOpen)
			{
				ConsoleHandler.Instance.WriteLocal("Console>");
				string command = Console.ReadLine();
				ConsoleHandler.Instance.ExecuteCommand("Console", command);
			}

			Console.WriteLine("Cleaning Game server...");
			DedicatedGameServer.Clean();

			Console.WriteLine("Closing the webserver...");
			webServer.Stop();
		}

	}
}
