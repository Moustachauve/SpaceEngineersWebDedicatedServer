
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
            Console.WriteLine("Initializing...");

			WebServer webServer = new WebServer();
			webServer.Start();

			Console.WriteLine("Server started.");


            Console.ReadLine();


            DedicatedGameServer.Clean();
		}

    }
}
