using GameServer.Events;
using GameServer.Hubs;
using GameServer.Timers;
using Microsoft.AspNet.SignalR;
using Sandbox;
using Sandbox.Engine.Utils;
using Sandbox.Game;
using Sandbox.Game.World;
using SpaceEngineers.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRage.Dedicated;
using VRage.FileSystem;
using VRage.Game;
using VRage.Game.SessionComponents;
using VRage.Utils;
using VRageRender;

namespace GameServer
{
	public static class DedicatedGameServer
	{
		public static EventHandler<ServerStatusChangedArgs> ServerStatusChanged;
		public static EventHandler<ServerConfigChangedArgs> ServerConfigChanged;

		static Thread serverThread;
		static VRage.Win32.WinApi.ConsoleEventHandler consoleHandler;

		private static bool IsRunning { get; set; }
		private static bool IsStopping { get; set; }
		private static bool IsReady { get { return IsRunning && !IsStopping && MySandboxGame.Static != null && MySandboxGame.Static.IsFirstUpdateDone; } }

		public static string SavePath { get; private set; }

		private static ServerStatus previousStatus;
		public static ServerStatus Status
		{
			get
			{
				NotifyStatusChanged();
				return GetCurrentServerStatus();
			}
		}

		private static MyConfigDedicated<MyObjectBuilder_SessionSettings> serverConfig;
		public static MyConfigDedicated<MyObjectBuilder_SessionSettings> ServerConfig
		{
			get
			{
				if (serverConfig == null)
				{
					ReloadServerConfig();
				}

				return serverConfig;
			}
		}

		static DedicatedGameServer()
		{
			MyFileSystem.Reset();
			SpaceEngineersGame.SetupBasicGameInfo();
			SpaceEngineersGame.SetupPerGameSettings();
			MySandboxGame.IsDedicated = true;

			MyPerGameSettings.SendLogToKeen = DedicatedServer.SendLogToKeen;

			MyPerServerSettings.GameName = MyPerGameSettings.GameName;
			MyPerServerSettings.GameNameSafe = MyPerGameSettings.GameNameSafe;
			MyPerServerSettings.GameDSName = MyPerServerSettings.GameNameSafe + "Dedicated";
			MyPerServerSettings.GameDSDescription = "Your place for space engineering, destruction and exploring.";

			MySessionComponentExtDebug.ForceDisable = true;

			MyPerServerSettings.AppId = 244850;

			SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MyPerServerSettings.GameDSName);
		}

		public static void Start()
		{
			if (IsRunning)
				return;

			IsRunning = true;
			serverThread = new Thread(RunMain) { IsBackground = true };
			serverThread.Start();
			NotifyStatusChanged();

			//Wait until the server is fully ready
			System.Threading.SpinWait.SpinUntil(() => IsReady);
			NotifyStatusChanged();
		}

		public static void Stop()
		{
			if (!IsReady || IsStopping)
				return;

			IsStopping = true;
			MySandboxGame.ExitThreadSafe();
			NotifyStatusChanged();
		}

		public static void Clean()
		{
			MyInitializer.InvokeAfterRun();
		}

		private static ServerStatus GetCurrentServerStatus()
		{
			if (IsReady)
			{
				return ServerStatus.Started;
			}
			else if (IsStopping)
			{
				return ServerStatus.Stopping;
			}
			else if (IsRunning)
			{
				return ServerStatus.Starting;
			}
			else
			{
				return ServerStatus.Stopped;
			}

		}

		private static void NotifyStatusChanged()
		{
			ServerStatus currentStatus = GetCurrentServerStatus();
			if (currentStatus == previousStatus)
				return;

			ServerStatus oldStatus = previousStatus;
			previousStatus = currentStatus;

			ServerStatusChanged?.Invoke(null, new ServerStatusChangedArgs(oldStatus, currentStatus));
			GlobalHost.ConnectionManager.GetHubContext<ServerHub>().Clients.All.updateStatus(currentStatus.ToString());
		}

		private static void StartServer()
		{
			try
			{
				RunMain();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private static void RunMain()
		{
			//ConfigForm<MyObjectBuilder_SessionSettings>.LogoImage = SpaceEngineersDedicated.Properties.Resources.SpaceEngineersDSLogo;
			ConfigForm<MyObjectBuilder_SessionSettings>.GameAttributes = Game.SpaceEngineers;
			ConfigForm<MyObjectBuilder_SessionSettings>.OnReset = delegate
			{
				SpaceEngineersGame.SetupBasicGameInfo();
				SpaceEngineersGame.SetupPerGameSettings();
			};
			MyFinalBuildConstants.APP_VERSION = MyPerGameSettings.BasicGameInfo.GameVersion;

			Console.WriteLine("Starting game server...");


			MySandboxGame.IsConsoleVisible = true;
			VRage.Win32.WinApi.AllocConsole();
			consoleHandler += new VRage.Win32.WinApi.ConsoleEventHandler(Handler);
			VRage.Win32.WinApi.SetConsoleCtrlHandler(consoleHandler, true);

			VRage.Service.ExitListenerSTA.OnExit += delegate
			{
				if (MySandboxGame.Static != null)
					MySandboxGame.Static.Exit();
			};

			Console.WriteLine(MyPerServerSettings.GameName + "  " + MyFinalBuildConstants.APP_VERSION_STRING);
			Console.WriteLine(String.Format("Is official: {0} {1}", MyFinalBuildConstants.IS_OFFICIAL, (MyObfuscation.Enabled ? "[O]" : "[NO]")));
			Console.WriteLine("Environment.Is64BitProcess: " + Environment.Is64BitProcess);

			try
			{
				Console.WriteLine("Path already initialized, content path: " + MyFileSystem.ContentPath);
				MySandboxGame.IsReloading = true;
			}
			catch (InvalidOperationException)
			{
				MyInitializer.InvokeBeforeRun(
				MyPerServerSettings.AppId,
				MyPerServerSettings.GameDSName,
				SavePath, DedicatedServer.AddDateToLog);
			}

			do
			{
				IsRunning = true;
				RunInternal();
			}
			while (MySandboxGame.IsReloading);
		}

		private static void RunInternal()
		{
			if (!MySandboxGame.IsReloading)
				MyFileSystem.InitUserSpecific(null);


			MySandboxGame.IsReloading = false;

			MyRenderProxy.Initialize(MySandboxGame.IsDedicated ? (IMyRender)new MyNullRender() : new MyDX11Render());
			MyFinalBuildConstants.APP_VERSION = MyPerGameSettings.BasicGameInfo.GameVersion;

			using (MySteamService steamService = new MySteamService(MySandboxGame.IsDedicated, MyPerServerSettings.AppId))
			{
				bool startGame = true;

				if (!steamService.HasGameServer)
				{
					MyLog.Default.WriteLineAndConsole("Steam service is not running! Please reinstall dedicated server.");
					startGame = false;
				}

				if (startGame)
				{
					VRageGameServices services = new VRageGameServices(steamService);

					using (MySandboxGame game = new MySandboxGame(services, Environment.GetCommandLineArgs().Skip(1).ToArray()))
					{
						MyRenderProxy.GetRenderProfiler().EndProfilingBlock();
						MyRenderProxy.GetRenderProfiler().EndProfilingBlock();

						game.Run();
					}
				}

				IsRunning = false;
				IsStopping = false;
				NotifyStatusChanged();

				if (MySandboxGame.IsConsoleVisible && !MySandboxGame.IsReloading && !Console.IsInputRedirected)
				{
					Console.WriteLine("Server stopped.");
				}
			}
		}



		public static bool GameAction(Action action)
		{
			if (serverThread == Thread.CurrentThread)
				throw new ThreadStateException("Invoking action on game thread from inside game thread! This will freeze the server!");

			try
			{
				AutoResetEvent e = new AutoResetEvent(false);

				MySandboxGame.Static.Invoke(() =>
				{
					/*if (m_gameThread == null)
					{
						m_gameThread = Thread.CurrentThread;
					}*/

					action();
					e.Set();
				});

				e.WaitOne();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public static MyConfigDedicated<MyObjectBuilder_SessionSettings> ReloadServerConfig()
		{
			string configFilePath = Path.Combine(SavePath, "SpaceEngineers-Dedicated.cfg");
			if (File.Exists(configFilePath))
			{
				MyConfigDedicated<MyObjectBuilder_SessionSettings> config = new MyConfigDedicated<MyObjectBuilder_SessionSettings>("SpaceEngineers-Dedicated.cfg");
				if (config == null)
					throw new FileLoadException("Failed to load server config at \"" + configFilePath + "\"");

				serverConfig = config;

				//Maybe we should check if the file really did changed?
				ServerConfigChanged?.Invoke(null, new ServerConfigChangedArgs(config));

				return config;
			}
			else
			{
				Console.WriteLine("Server config not found at \"" + configFilePath + "\"");
			}

			return null;
		}

		private static bool Handler(VRage.Win32.WinApi.CtrlType sig)
		{
			switch (sig)
			{
				case VRage.Win32.WinApi.CtrlType.CTRL_SHUTDOWN_EVENT:
				case VRage.Win32.WinApi.CtrlType.CTRL_CLOSE_EVENT:
					{
						MySandboxGame.Static.Exit();
						return false;
					}
				default:
					break;
			}
			return true;
		}

	}
}
