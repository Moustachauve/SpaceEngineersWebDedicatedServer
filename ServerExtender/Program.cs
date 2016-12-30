using Sandbox;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Engine.Utils;
using Sandbox.Game;
using Sandbox.Game.Entities;
using SpaceEngineers.Game;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using VRage.Dedicated;
using VRage.FileSystem;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.SessionComponents;
using VRage.Service;
using VRage.Utils;
using VRageRender;

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




			/*GameServer server = new GameServer();
			server.Start();

			while(MySandboxGame.Static == null || !MySandboxGame.Static.IsFirstUpdateDone)
			{
				Thread.Sleep(500);
			}

			MyEntity[] entities = new MyEntity[0];
			server.GameAction(() => entities = MyEntities.GetEntities().ToArray());

			foreach(var entity in entities)
			{
				Console.WriteLine(entity.DisplayName);
			}

			server.Stop();


			Console.WriteLine("Hello");*/
			Console.ReadKey(false);

			GameServer.Clean();
		}

    }
}
