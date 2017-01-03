using GameServer.Timers;
using Sandbox;
using Sandbox.Engine.Utils;
using Sandbox.Game;
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
        static Thread serverThread;
        static VRage.Win32.WinApi.ConsoleEventHandler consoleHandler;

        private static ServerEvents serverEvents;
        public static ServerEvents ServerEvents
        {
            get
            {
                if (serverEvents == null)
                {
                    serverEvents = new ServerEvents();
                }

                return serverEvents;
            }
        }
        
        public static bool IsRunning { get; private set; }
        public static bool IsReady { get { return IsRunning && MySandboxGame.Static != null && MySandboxGame.Static.IsFirstUpdateDone; } }

        public static void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            serverThread = new Thread(RunMain) { IsBackground = true };
            serverThread.Start();
        }

        public static void Stop()
        {
            if (!IsReady)
                return;

            MySandboxGame.ExitThreadSafe();
        }

        public static string GetStatus()
        {
            if (DedicatedGameServer.IsReady)
            {
                return "started";
            }
            else if (DedicatedGameServer.IsRunning)
            {
                return "starting";
            }
            else
            {
                return "stopped";
            }
        }

        public static void Clean()
        {
            MyInitializer.InvokeAfterRun();
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

            //ConfigForm<MyObjectBuilder_SessionSettings>.LogoImage = SpaceEngineersDedicated.Properties.Resources.SpaceEngineersDSLogo;
            ConfigForm<MyObjectBuilder_SessionSettings>.GameAttributes = Game.SpaceEngineers;
            ConfigForm<MyObjectBuilder_SessionSettings>.OnReset = delegate
            {
                SpaceEngineersGame.SetupBasicGameInfo();
                SpaceEngineersGame.SetupPerGameSettings();
            };
            MyFinalBuildConstants.APP_VERSION = MyPerGameSettings.BasicGameInfo.GameVersion;

            Console.WriteLine("Starting server...");


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


            string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MyPerServerSettings.GameDSName);

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
                dataPath, DedicatedServer.AddDateToLog);
            }


            Console.WriteLine(MySandboxGame.ConfigDedicated.ServerName);


            do
            {
                RunInternal();
            } while (MySandboxGame.IsReloading);

            IsRunning = false;
            Console.WriteLine("FirstUpdate: " + MySandboxGame.Static.IsFirstUpdateDone);
        }

        private static void RunInternal()
        {
            if (!MySandboxGame.IsReloading)
                MyFileSystem.InitUserSpecific(null);


            MySandboxGame.IsReloading = false;

            VRageRender.MyRenderProxy.Initialize(MySandboxGame.IsDedicated ? (IMyRender)new MyNullRender() : new MyDX11Render());
            MyFinalBuildConstants.APP_VERSION = MyPerGameSettings.BasicGameInfo.GameVersion;

            using (MySteamService steamService = new MySteamService(MySandboxGame.IsDedicated, MyPerServerSettings.AppId))
            {
                if (!steamService.HasGameServer)
                {
                    MyLog.Default.WriteLineAndConsole("Steam service is not running! Please reinstall dedicated server.");
                    return;
                }

                VRageGameServices services = new VRageGameServices(steamService);

                using (MySandboxGame game = new MySandboxGame(services, Environment.GetCommandLineArgs().Skip(1).ToArray()))
                {
                    VRageRender.MyRenderProxy.GetRenderProfiler().EndProfilingBlock();
                    VRageRender.MyRenderProxy.GetRenderProfiler().EndProfilingBlock();

                    game.Run();
                }

                if (MySandboxGame.IsConsoleVisible && !MySandboxGame.IsReloading && !Console.IsInputRedirected)
                {
                    Console.WriteLine("Server stopped.");
                }
            }
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
    }
}
